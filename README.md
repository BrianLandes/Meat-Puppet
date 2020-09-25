# Meat-Puppet
A Unity package for controlling and animating 3D humanoid characters, especially NPCs.

### Description
Meat Puppet is a Unity asset that quickly and easily gives humanoid characters life. It can be applied to any 3D model and handles animating them and moving them around the environment.

### Setup Is Easy
1. Add the 'Meat Puppet Manager' prefab to the scene.
2. Add the 'MeatPuppet' component to any character (alongside the Animator component).

* Bake Unity Navigation Mesh.
* (Optional) Give the MeatPuppet a destination.
* (Optional) Add player controller components to the Meat Puppet for player control.

### Features
* Animates various states of locomotion, including walking, running, strafing, and jumping.

![Animation Controller](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Animation%20Controller.png?raw=true)
![Locomotion](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Locomotion.gif?raw=true)

* A character can be given a transform, a world position, or a direction and they will navigate and move to the destination.

![Navigation](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Navigation.png?raw=true)

* Meat Puppet uses physics to move the character, which results in dynamic and interesting collisions with other characters and objects.

![Collision](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Collision.gif?raw=true)

* A unique 'Capsule-and-spring' design allows the character to 'glide' over uneven terrain, stairs, and slopes.

![Uneven Terrain](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Uneven%20Terrain.gif?raw=true)
![Stairs](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Steps.gif?raw=true)
![Slopes](https://github.com/BrianLandes/Meat-Puppet/blob/master/Page_Images/Slopes.gif?raw=true)
