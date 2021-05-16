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
Solve g, f(t)=h(eight)
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

#TODOs
- [ ] Investigate QuadTree
- [ ] Enforce player is inside world by letting the world add colliders around it's boundary
- [ ] Move game specific functionaliy to Game class