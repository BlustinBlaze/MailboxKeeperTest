import RPi.GPIO as GPIO
import time
import requests
from gpiozero import DistanceSensor

# Configurer la bibliothèque RPi.GPIO
GPIO.setmode(GPIO.BCM)  # Utiliser la numérotation BCM
GPIO.setup(22, GPIO.IN, pull_up_down=GPIO.PUD_UP)  # Configurer la broche 22 comme entrée avec pull-up

API_BASE_URL = 'http://137.184.171.170:80'
LOGIN_ENDPOINT = f'{API_BASE_URL}/v1/mailbox/login'
UPDATE_ENDPOINT = f'{API_BASE_URL}/v1/mailbox'

USERNAME = "1"
PASSWORD = "test2"

# Initialiser le capteur ultrasonique
sensor = DistanceSensor(trigger=18, echo=24)

# Variables initiales
last_empty = None  # État précédent de la boîte aux lettres (vide ou pleine)
last_state = None  # État précédent de l'interrupteur Reed
jwt_token = None   # Token d'authentification JWT

# Fonction pour se connecter et obtenir le JWT
def login_get_jwt():
    login_data = {
        "id": USERNAME,
        "password": PASSWORD
    }
    try:
        response = requests.post(LOGIN_ENDPOINT, json=login_data)
        if response.status_code == 200:
            token = response.json().get('token')  # Adapter selon la réponse de l'API
            print(f"Connexion réussie. Jeton JWT reçu : {token}")
            return token
        else:
            print(f"Échec de la connexion : {response.status_code}")
            return None
    except Exception as e:
        print(f"Une erreur s'est produite lors de la connexion : {e}")
        return None

# Fonction pour mettre à jour l'état de la boîte aux lettres sur le serveur
def update_mailbox_status(jwt_token, is_open, is_empty):
    headers = {
        'Authorization': f'Bearer {jwt_token}',
        'Content-Type': 'application/json'
    }
    data = {
        "id": USERNAME,
        "status": is_open,  # True si ouvert, False si fermé
        "mailWeight": is_empty  # 1 si vide, 0 si plein
    }
    try:
        response = requests.put(UPDATE_ENDPOINT, json=data, headers=headers)
        if response.status_code == 200:
            print(f"État de la boîte mis à jour avec succès : {'Ouvert' if is_open else 'Fermé'}, {'Vide' if is_empty else 'Plein'}")
        else:
            print(f"Échec de la mise à jour de l'état de la boîte : {response.status_code}")
    except Exception as e:
        print(f"Une erreur s'est produite lors de la mise à jour de l'état de la boîte : {e}")

# Programme principal
try:
    jwt_token = login_get_jwt()
    if not jwt_token:
        print("Erreur : Impossible d'obtenir le jeton JWT. Vérifiez les identifiants ou la connexion.")
        exit()

    # Lire l'état initial de l'interrupteur Reed
    last_state = GPIO.input(22)
    print('Prêt à détecter les changements dans l’interrupteur Reed et le capteur ultrasonique')

    while True:
        # Lire l'état actuel de l'interrupteur Reed
        current_state = GPIO.input(22)

        # Vérifier si l'état de l'interrupteur Reed a changé
        if current_state != last_state:
            status = "Open" if current_state == GPIO.HIGH else "Closed"
            update_mailbox_status(jwt_token, status, last_empty)
            last_state = current_state  # Mettre à jour l'état précédent
            print(f"Interrupteur {'ouvert' if status == 'Open' else 'fermé'} détecté.")

        # Vérifier l'état du capteur ultrasonique seulement si la boîte est fermée
        if last_state == GPIO.LOW:  # Seulement si fermé
            distance = sensor.distance  # Obtenir la distance en mètres
            is_empty = 0 if distance >= 0.2 else 1  # Déterminer si vide (1) ou plein (0)

            # Mettre à jour le serveur si l'état change (vide ou plein)
            if is_empty != last_empty:
                update_mailbox_status(jwt_token, "Closed", is_empty)
                last_empty = is_empty  # Mettre à jour l'état précédent
                print(f"État de la boîte : {'Vide' if is_empty else 'Plein'} détecté.")

        time.sleep(0.5)  # Petit délai pour éviter de saturer le réseau

except KeyboardInterrupt:
    print("Programme arrêté")
finally:
    GPIO.cleanup()  # Nettoyer les broches GPIO à la fin
