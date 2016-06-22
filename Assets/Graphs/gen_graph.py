#!/usr/bin/env python
# -*- coding: utf8 -*-
from __future__ import division, print_function
from math import sin, cos, pi, sqrt
import json

def distance(v1,v2):
    x1,y1,z1 = v1
    x2,y2,z2 = v2

    return sqrt((x1-x2)**2 + (y1-y2)**2 + (z1-z2)**2)

def gen_torus_graph(radius_ext, radius_int, n_ext, n_int):
    ris = []
    for n1 in range(n_ext):
        angle1 = n1 / n_ext * 2 * pi
        v1 = (cos(angle1) * radius_ext, sin(angle1) * radius_ext, 0)

        circle_slice = []
        for n2 in range(n_int):
            angle2 = n2 / n_int * 2 * pi
            v2 = (cos(angle2), 0, sin(angle2))
            v2 = (
                v2[0]*cos(angle1) - v2[1]*sin(angle1),
                v2[0]*sin(angle1) + v2[1]*cos(angle1),
                v2[2])
            v2 = tuple(v * radius_int for v in v2)


            circle_slice.append(tuple(x1+x2 for x1,x2 in zip(v1,v2)))
        ris.append(circle_slice)


    asd = {'nodes':[], 'edges':[]}
    for i, c in enumerate(ris):
        for j, p in enumerate(c):
            asd['nodes'].append({'name':'{}-{}'.format(i,j),'group':1})

            # link with next node on same ring
            asd['edges'].append({
                'source':i*n_int+j,
                'target':i*n_int+(j+1)%n_int,
                'value':distance(ris[i][j], ris[i][(j+1)%n_int]),
                })

            # link with next node on next ring
            asd['edges'].append({
                'source':i*n_int+j,
                'target':(i+1)%n_ext*n_int+j,
                'value':distance(ris[i][j], ris[(i+1)%n_ext][j]),
                })

            # link with next node on next ring and next position
            asd['edges'].append({
                'source':i*n_int+j,
                'target':(i+1)%n_ext*n_int+(j+1)%n_int,
                'value':distance(ris[i][j], ris[(i+1)%n_ext][(j+1)%n_int]),
                })

    asd['parameters'] = {
        'stiffness': 300,
        'repulsion': 400,
        'convergence': 0.7,
        'damping': 0.5,
    }

    from pprint import pprint
    pprint(asd)
    return json.dumps(asd)



asd = gen_torus_graph(30, 6, 12, 12)
with open('Torus.json', 'w') as f:
    f.write(asd)

