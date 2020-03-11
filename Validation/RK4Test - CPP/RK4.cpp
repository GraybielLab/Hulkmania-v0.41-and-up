#include "stdafx.h"
#include "rk4.h"

RungaKutta4::State::State(float ang, float vel) : angle(ang), velocity(vel) { } 
			
RungaKutta4::Derivative::Derivative(float dang, float dvel) : dangle(dang), dvelocity(dvel) { }


RungaKutta4::Derivative RungaKutta4::_evaluate(const RungaKutta4::State &initial, float t, ComputeAccelerationFn fn){
	Derivative output;
	output.dangle = initial.velocity;
	output.dvelocity = fn(initial, t);
	return output;
}

RungaKutta4::Derivative RungaKutta4::_evaluate(const RungaKutta4::State &initial, float t, float dt, const RungaKutta4::Derivative &d, ComputeAccelerationFn fn){
	State state;
	state.angle = initial.angle + d.dangle*dt;
	state.velocity = initial.velocity + d.dvelocity*dt;
	Derivative output;
	output.dangle = state.velocity;
	output.dvelocity = fn(state, t+dt);
	return output;
}

			
void RungaKutta4::integrate(RungaKutta4::State &state, float t, float dt, ComputeAccelerationFn fn){
	Derivative a = _evaluate(state, t, fn);
	Derivative b = _evaluate(state, t, dt*0.5f, a, fn);
	Derivative c = _evaluate(state, t, dt*0.5f, b, fn);
	Derivative d = _evaluate(state, t, dt, c, fn);

	const float dadt = 1.0f/6.0f * (a.dangle + 2.0f*(b.dangle + c.dangle) + d.dangle);
	const float dvdt = 1.0f/6.0f * (a.dvelocity + 2.0f*(b.dvelocity + c.dvelocity) + d.dvelocity);
	
	state.angle = state.angle + dadt*dt;
	state.velocity = state.velocity + dvdt*dt;
}
	