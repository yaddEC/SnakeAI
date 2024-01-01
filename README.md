# Projet Snake IA:Reward Learning/Algorithmes G�n�tiques

![](001.gif)

## I. Pr�sentation du Projet

Ce projet vise � d�velopper un jeu Snake intelligent o� le snake est contr�l� par une intelligence artificielle qui apprend et �volue gr�ce � des techniques d'apprentissage par renforcement (Reinforcement Learning - RL) et des algorithmes g�n�tiques. Le but est de cr�er un snake capable de prendre des d�cisions complexes pour survivre le plus longtemps possible tout en mangeant de la nourriture.

## II. M�canismes d'IA Impl�ment�s

### Apprentissage et Mise � Jour des Poids :
#### Feedforward :
Les donn�es d'entr�e traversent le r�seau de l'entr�e � la sortie, permettant au snake de prendre une d�cision bas�e sur son state/weight actuel.

#### Reward and Punishment :
Apr�s chaque action, une r�compense (ou une p�nalit�) est calcul�e en fonction de si le snake s'est rapproch� de la nourriture, s'il a mang� ou s'il est mort. Cela aide � guider l'apprentissage.
#### Mise � jour par Q-Learning :
Utilise l'algorithme Q-Learning pour mettre � jour les poids des perceptrons. Le r�seau pr�dit les valeurs Q actuelles et futures, calcule l'erreur (TD-Error) pour l'action prise, puis ajuste les poids pour minimiser cette erreur.

### �volution et Mutation :
#### S�lection et Croisement :
� chaque g�n�ration, les snakes les plus performants sont s�lectionn�s. Leurs poids sont crois�s pour cr�er une nouvelle g�n�ration, introduisant de la variabilit� et de meilleures strat�gies.
#### Mutation : 
Avec une petite probabilit�, les poids des nouveaux snakes sont l�g�rement modifi�s pour introduire davantage de diversit� g�n�tique et �viter de rester bloqu� dans des "local optima".

## III. Contr�les 

- **Snakes et environnement:** La plupart des valeurs pouvant �tre chang�s pour la g�n�ration des snakes sont dans le GameManager : Le nombre de snake g�n�r�, leurs vitesse, la limite de temps imparti, et m�me le score minimum � partir duquel les snakes sont visibles sont modifiable depuis l'inspecteur. La taille de la grille de jeu est en statique et doit �tre modifi� dans le script.

- **Nombre d'inputs/outputs:** Le nombre d'input, d'hidden input et d'output sont directement modifiable dans le script du MLPNetwork. ***Attention, la save actuel ne marchera plus si le nombre d'input ne correspond pas � ceux enregistrer***. L'IA entrain� est � :

    public int numInputPerceptrons = 24;


    public int numHiddenPerceptrons = 30;


    public int numOutputPerceptrons = 4;

- **Save:** La save est nomm� *"savefile.json"* et doit se trouver dans *"Assets/Save"*. Elle sera automatiquement load�, ou g�n�rer, si elle n'existe pas. La save pr�-entrain� se trouve dans *"Assets/Save/final_trained_ai"*.


## IV. Probl�mes Rencontr�s et Solutions Apport�es

1. **Stall et Boucles:** Les snakes entraient dans des boucles infinies pour survivre sans chercher de nourriture.Ensuite, les snakes se collaient � la nourriture sans la manger. 
Dans ces deux cas, j'ai d� ajuster les r�compenses pour encourager l'exploration et la consommation de nourriture. Plus il attends sans avoir pris de fruit, plus il sera p�nalis�.

2. **Nombre d'Outputs Insuffisant:** Le snake ne disposait pas de suffisamment d'options pour prendre des d�cisions complexes. Je pensais avoir initialement besoin que d'un seul float, allant de -1 � 1. Et le snake devait tourner � droite, a gauche ou continuer tout droit selon le r�sultat de ce float. Mais cela ne donnait que trop peu de variabilit� au mouvement du snake. J'ai donc augmenter le nombre de neurones de sortie pour permettre une plus grande vari�t� de d�cisions.

3. **Finetuning:** Afin d'avoir un comportement qui s'am�liorer avec les g�n�rations, j'ai d� modifier de mai�re empirique le score de fitness et le reward des snakes, ce qui est assez fastidieux.

4. **Inputs non pris en compte:** Les valeurs de mes inputs �taient trop complexes(position du snake/de son corps/fruit en bruts), mon jeu ayant un aspect al�atoire assez pr�sent (spawn du fruit et des snakes), l'IA n'arrivait pas a faire sens de ces informations. J'ai donc modifi� plusieurs fois ces inputs, jusqu'� trouver les bons ( visions � 8 directions au niveau de la t�te du snake pour la nourriture, son corps, et le mur.)


## Sources:

- ["I Created The Perfect Snake AI With Reinforcement Learning" ](https://www.youtube.com/watch?v=FKE3fkDgI70)
- [AI Algorithm of Snake Game - Discussion on Reddit](https://www.reddit.com/r/programming/comments/5ly972/ai_algorithm_of_snake_game_share_opinions_if_you/)
- ["Neural Network Learns to Play Snake"](https://www.youtube.com/watch?v=zIkBYwdkuTk)
- ["Neural Network to Play Snake Game"](https://towardsdatascience.com/today-im-going-to-talk-about-a-small-practical-example-of-using-neural-networks-training-one-to-6b2cbd6efdb3)
- ["RL-GA: A Reinforcement Learning-Based Genetic Algorithm for Electromagnetic Detection Satellite Scheduling Problem" by Yanjie Song, Luona Wei, Qing Yang,Jian Wu,Lining Xing,Yingwu Chen](https://arxiv.org/pdf/2206.05694.pdf)
- ["Teaching AI to play Snake with Genetic Algorithm" by David Lyu](https://techs0uls.wordpress.com/2020/02/03/teaching-ai-to-play-snake-with-genetic-algorithm/)
- "Artificial Intelligence for Games Second Edition" by Ian Millington & John Funge




