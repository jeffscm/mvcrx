# MVC RX Package v3.0.0

Install this package to have full access to MVC RX framework.

Based on Previous work available here:
- https://github.com/jeffscm/mvcrx
- https://github.com/jeffscm/mvcc2
- https://github.com/jeffscm/mvccunity

# Contributors

- Mattijs Driel
- Geert Lugtenberg

## Requirements

+ Unity 2019 and higher

## Features included in this version:

+ Controller

    - AppController: Scriptable Controllers where you can create callback to listen to events and react
        - Use cases : Logic involving other Scriptable Objects, Logic, Network access, External packages access
    - AppMonoController: Controller based on MonoBahvior, access other scripts and GameObjects from this type of controller
        - Use cases : Access 3DViews, GameObjects inside the hierarch, custom Logic
    - UIViewController: Controller for Unity UI System, place it under root of your group of Views to control them
        - Use cases : Control and restrict access to UI GameObject elemets. Keep logic away from UI.

+ Model
    - Data Container Models: app.model has the access to data containers where you can store your data instead leaving it inside Controllers/Views/Others.
    - View Models: Attached to UIView elements and they are custom to each view. Use the access from UIViewController classes in order to set its values.

+ View
    - UIView : Unity UI system base View class where you reference GameObjects in the Unity Editor side. It is connected to UIViewController classes. It has custom animation system, you can use your own animations.

+ 3D View
    - UI3DView : Wrapper to handle multiple views for 3D content, it has no animation system. It is connected to AppMonoController and AppController classes.

