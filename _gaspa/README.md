
#Classi

    ParticleEngine
    - particles: list
    - springs: list
    - update(dt)

    IParticle
    - position:Vector
    - velocity:Vector
    - force:Vector
    - mass:Real

    Spring
    - particle1:Particle
    - particle2:Particle
    - length:Real
    - stiffness:Real


    IModel
    - getNodes()
    - distance(node1, node2)
    - adiacencyList(node)

    INode
    - visualName
    - data

#Misc

##Abstract Model
non sapendo come ci verranno forniti i dati sarebe da definire un interfaccia per ricavare info dal modello tra cui:

- id, nome e dati dei nodi
- distanza fra due nodi
- nodi adiacenti a un dato nodo (rispetto a una certa distanza)

##Passaggio 2D/3D realtime
e' possibile azzerare l'asse z
e integrare usando soltanto x e y
cambiando da proiezione prospettica a proiezione ortogonale

per tornare indietro basta rimettere la proiezione prospettica, randomizzare leggermente la posizione in z di ogni punto e tornare a integrare in 3d

