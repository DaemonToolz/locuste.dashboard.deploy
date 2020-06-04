# locuste.dashboard.deploy
Locuste - Outil de déploiement et de mise à jour


Le project Locuste se divise en 4 grandes sections : 
* Automate (Drone Automata) PYTHON (https://github.com/DaemonToolz/locuste.drone.automata)
* Unité de contrôle (Brain) GOLANG (https://github.com/DaemonToolz/locuste.service.brain)
* Unité de planification de vol / Ordonanceur (Scheduler) GOLANG (https://github.com/DaemonToolz/locuste.service.osm)
* Interface graphique (UI) ANGULAR (https://github.com/DaemonToolz/locuste.dashboard.ui)

![Composants](https://user-images.githubusercontent.com/6602774/83644711-dcc65000-a5b1-11ea-8661-977931bb6a9c.png)

Tout le système est embarqué sur une carte Raspberry PI 4B+, Raspbian BUSTER.
* Golang 1.11.2
* Angular 9
* Python 3.7
* Dépendance forte avec la SDK OLYMPE PARROT : (https://developer.parrot.com/docs/olympe/, https://github.com/Parrot-Developers/olympe)

![Vue globale](https://user-images.githubusercontent.com/6602774/83644783-f10a4d00-a5b1-11ea-8fed-80c3b76f1b00.png)

Détail des choix techniques pour la partie Outil de diagnostique :

* [UWP] - Facilité de déploiement et de distribution sur le Microsoft Store, l'environnement sandboxé permet une sécurité renforcée, surtout au moment du chargement / téléchargement des fichiers. 
* [Socket IO] - Intégration facile avec l'environnement Socket IO du Golang, permet d'avoir un suivi des événements.

Evolutions à venir : 
* Amélioration de la GUI
* Ajout du support multi-fichier
* Fix et correctifs de sécurité
* Démarrage / Arrêt des applications déployées
