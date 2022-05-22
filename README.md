# platformtest

Platform test, is a test of creating a simple game using win2d. Components are classic tiles and animated sprites.

# The Game
Plan to make a platform game, one level. Tools for creating the map is Tiled.

# Gravity and jumping
Formula: f(t)=1/2*g*t^2+v0*t+p0
Variables: g=gravity constant, t=time, v0=initial speed, p0=initial position
## Calculate initial speed
Solve v0 when gravity and initial speed is equal(when parabolica is flat, the tangent, the top of the parabolica)
f'(t)=g*t+v0 => v0=-g*t
## Calculate gravity
Solve g, f(t)=h(height)
f(t)=1/2*g*t^2+v0*t+p0 => insert v0=-g*t, p0=0 => h=1/2*g*t^2+(-g*t)*t+0 => h=-1/2*g*t^2 => g=-2*h/t^2
## v0 with height
v0=-g*t => insert solved g, g=-2*h/t^2 => v0=-(-2*h/t^2)*t => v0=2h/t
## Jump and run
v0=2*h*vx/x, g=-2*h*vx^2/x^2
## Integration
### Euler
pos += vel * dt
vel += acc * dt
### Velocity Verlet
pos += vel * dt + 1/2 * acc * dt * dt
new_acc = f(pos)
vel += 1/2(acc + new_acc) * dt
acc = new_acc

### Runge-Kutta

# TODOs
- [ ] Investigate QuadTree
- [x] Enforce player is inside world by letting the world add colliders around it's boundary
- [x] Move game specific functionaliy to Game class
- [ ] Move generic parts of the engine to library and make more parts generic
- [ ] Improve and extend particles
- [ ] Hud
- [ ] Enemy
- [ ] AI
- [ ] Fight the enemy
- [ ] Hud part II

# Licenses
## NUnit
Copyright (c) 2021 Charlie Poole, Rob Prouse

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
## Newtonsoft
The MIT License (MIT)

Copyright (c) 2007 James Newton-King

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
## FakeIt
The MIT License (MIT)

Copyright (c) FakeItEasy contributors. (fakeiteasy@hagne.se)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
