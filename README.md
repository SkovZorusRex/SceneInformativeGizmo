# SceneInformativeGizmo

[English](#English) / [Français](#Français)

# English

## This tool can :

1. This tool is an EditorWindow that can be opened in two ways : <br />
   - Using the tabs Window > Custom > Show Gizmos <br />
   - When clicking on a SceneGizmoAsset scriptable object, the inspector will show a Button to open this Editor Window using its Gizmo struct information <br />
2. The EditorWindow display the different Gizmo positions with their text <br />
3. The scene view display all these gizmos as white spheres with the text description in black <br />
4. The gizmos only appears in the Scene View, not in the Game View <br />
5. You can select a gizmo to edit its position <br />
6. The selected gizmo can be moved directly from the scene view : using the classic unity tool to move an object <br />
7. All changes are directly saved in the SceneGizmoAsset without user action. <br />
8. You can ctrl+z to remove the last changes on the gizmos <br />
9. Right-clicking on the gizmo make a menu appears that permits to <br />
   - Reset the gizmo position to the one prior the edition <br />
   - Delete a gizmo <br />
   
## Missing Features

- Deleting Gizmo will **only** works **visually**, the scriptable object will not change

## About The Project

- All scripts can be found in **Assets/Editor/**

This tool use **SerializedObject** and **SerializedProperty** as it allow to use the Undo/Redo and automatically save changes. <br />
While it's working, performance are not specifically watched, so expect unoptimized code. <br />
Using a class to keep track of each gizmo related data is a simple way to configure each gizmo in a loop. Originally it was a struct but struct are copied not passed by reference. <br />

# Français

## Cet outil peut :

1. Cet outil est un EditorWindow qui peut être ouverte de deux façons : <br />
   - En utilisant les onglets Window > Custom > Show Gizmos. <br />
   - Lorsque vous cliquez sur un Scriptable Object SceneGizmoAsset, l'inspecteur affiche un bouton permettant d'ouvrir l'Editor Window en utilisant les informations de sa structure Gizmo. <br />
2. La fenêtre de l'éditeur affiche les différentes positions du Gizmo avec leur texte. <br />
3. La vue de la scène affiche tous ces gizmos sous forme de sphères blanches avec le texte de description en noir. <br />
4. Les gizmos n'apparaissent que sur la scène, pas en jeu. <br />
5. Vous pouvez sélectionner un gizmo pour modifier sa position. <br />
6. Le gizmo sélectionné peut être déplacé directement dans la scène : en utilisant l'outil de déplacement classique pour déplacer un objet. <br />
7. Toutes les modifications sont directement enregistrées dans le SceneGizmoAsset sans action de l'utilisateur. <br />
8. Vous pouvez faire ctrl+z pour supprimer les derniers changements sur les gizmos. <br />
9. Un clic droit sur le gizmo fait apparaître un menu qui permet de <br />
   - Remettre la position du gizmo à celle d'avant l'édition <br />
   - Supprimer un gizmo <br />
   
## Fonctionnalités manquantes

- Supprimer un Gizmo fonctionne **seulement** de manière **visuel**, le scriptable object ne change pas

## A Propos du Projet

- Tous les scripts peuvent être trouvés dans **Assets/Editor/**

Cet outil utilise **SerializedObject** et **SerializedProperty** car il permet d'utiliser le Undo/Redo et de sauvegarder automatiquement les modifications. <br />
Pendant que cela fonctionne, les performances ne sont pas spécifiquement surveillées, donc attendez-vous à un code non optimisé. <br />
L'utilisation d'une classe pour garder la trace des données relatives à chaque gizmo est un moyen simple de configurer chaque gizmo dans une boucle. À l'origine, il s'agissait d'une structure, mais les structures sont copiées et non transmises par référence. <br />
