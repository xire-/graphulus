# graphulus
An application to experiment with graphs exploration and virtual reality, made using [Unity](https://unity3d.com/) for the [Oculus Rift](https://www.oculus.com/) and the [Leap Motion Controller](https://www.leapmotion.com/).

Graphs are placed in the 3D virtual space using a [force-directed](https://en.wikipedia.org/wiki/Force-directed_graph_drawing) layout, and can be manipulated through hand gestures (i.e., single pinches to move nodes around and double pinches to move and scale the graph).

To start the application, play the scene `SceneVRLP.unity` (don't forget to tick `Virtual Reality Supported` in the Unity editor located in `Edit > Project Settings > Player`). If you don't have an Oculus or a Leap Motion device, you can play with the scene `Scene.unity`, where the graph can be controlled using mouse and keyboard.   
Both scenes have been tested with Unity v5.3.5f1, Oculus Rift DK2 with Runtime v1.5.0 and Leap Motion Orion v3.1.3.

Francesco and Marco


## Credits
- [Force-Directed Graph](https://bl.ocks.org/mbostock/4062045) (for a graph of encounters between the characters of Les Misérables, as compiled by [Donald Knuth](http://www-cs-faculty.stanford.edu/~uno/sgb.html))
- [d3-force](https://github.com/d3/d3-force)
- [The Barnes-Hut Algorithm](http://arborjs.org/docs/barnes-hut)
- [New Unity UI + OVR Look-Based Input HOWTO](https://forums.oculus.com/community/discussion/16710/new-unity-ui-ovr-look-based-input-howto) (for handling UI events with a VR device)


## License
The MIT License (MIT)

Copyright (c) 2016 Francesco Cagnin, Marco Gasparini

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
