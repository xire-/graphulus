# graphulus
Experiments about graphs exploration and virtual reality with [Unity](https://unity3d.com/), and the [Leap Motion Controller](https://www.leapmotion.com/).

## Setup Instructions
In an attempt to be more conformant with Leap Motion's licensing, the Core Assets are no longer directly included, you will need to acquire them yourself. After cloning the repository, you will need to [import the Leap Motion Core Assets](https://developer.leapmotion.com/releases/core-assets-440)

## Usage

Graphs are loaded from JSON files and placed in the 3D virtual space using a [force-directed](https://en.wikipedia.org/wiki/Force-directed_graph_drawing) layout. The application can be started in two different scenes:
- Play `Scene.unity` (used during development for testing purposes) if you don't have an Oculus Rift or a Leap Motion Controller. The camera can be controlled with the `W`/`A`/`S`/`D` keys and the mouse, but the graph cannot be manipulated. Settings can be altered by the following key bindings:
  - `R` toggles edges active
  - `T` toggles labels active
  - `N` toggles auto rotation
  - `B` increases auto rotation speed
  - `V` decreases auto rotation speed
  - `L` toggles theme
- Play `SceneVRLP.unity` for the VR experience. The camera is controlled by the Oculus Rift, and the graph can be manipulated by the Leap Motion Controller in two ways: use single pinches to move nodes around, and double pinch to move/scale/rotate the whole graph. Look down to activate the menu, and interact with its elements by gazing for a brief period of time. Do not forget to check `Edit > Project Settings > Player > Virtual Reality Supported` in the Unity editor.

This project has been developed and tested with Leap Motion Core Assets 4.4.0, and Unity 2017.4.1f1, however it should be compatible with any newer version of Unity that is out of the box compatible with the aforementioned Leap Core Assets.
It has also been tested using the SteamVR runtime and an HTC vive, but it should work with any VR Runtime and hardware that supports native Unity integration (including via package manager)

Francesco, Marco, and JCorvinus

## Credits
- [Force-Directed Graph](https://bl.ocks.org/mbostock/4062045) (for a sample graph on the characters of Les Misérables, as compiled by [Donald Knuth](http://www-cs-faculty.stanford.edu/~uno/sgb.html))
- [d3-force](https://github.com/d3/d3-force)
- [The Barnes-Hut Algorithm](http://arborjs.org/docs/barnes-hut)
- [New Unity UI + OVR Look-Based Input HOWTO](https://forums.oculus.com/community/discussion/16710/new-unity-ui-ovr-look-based-input-howto) (for handling UI events with a VR device)
- [Holos](holos.io) for updating the project to work with newer versions of Unity and Leap Motion


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
