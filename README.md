# FishNet-Multiplayer-Vehicle
Example was done in Unity 2022.3.11f1 with FishNet 4.0.6


https://github.com/dexsper/FishNet-Multiplayer-Vehicle/assets/51516115/1fd70be5-acf3-4bca-af7c-06e0c5577488


### Setup vehicle structure
![image](https://github.com/dexsper/FishNet-Multiplayer-Vehicle/assets/51516115/17451d37-e8fb-4789-a99a-72f14af51c9c)
![image](https://github.com/dexsper/FishNet-Multiplayer-Vehicle/assets/51516115/455f075d-76e9-474b-93fe-b38196e0aa48)

Steer Curve:
<br>
0.9s - 40 (value)
<br>
20s - 15 (value) 
<br>

`CollidersParent` is needed to store wheel colliders
<br>
`Visual` GameObject has child too because i need offset of my model (Offset is applied to Car2) 

If car have jitter when turn to sides: change your `CoM` in rigidbody
Edit your springs, for exmaple:<br>
Spring Value - 50000
Damper Value - 4500
<br>

Forward wheels will be have `friction` and `sideways friction`:
<br>
Stiffness Value - 2
