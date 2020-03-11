// RK4Test.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "RK4.h"
#include <math.h>
#include <conio.h>

//----------------------------------------------------------------------------------
static float K=0;

//----------------------------------------------------------------------------------
static float _calculateIPAcceleration(const RungaKutta4::State &state, float t){
	return K * sin(state.angle * 3.14159265453f / 180.0f); 
}

//----------------------------------------------------------------------------------
void testRK4(float startAngleDeg, float accelK, float timeStep){
	RungaKutta4::State state;

	K = accelK;
	float dt = timeStep;
	state.velocity = 0;
	state.angle = startAngleDeg;
	fprintf_s(stdout, "Using K=%f, velocity=%f, angle=%f, t=0, dt=%f\n", K, state.velocity, state.angle, dt);
	RungaKutta4::integrate(state, 0, 0.05f, &_calculateIPAcceleration);
	fprintf_s(stdout, "After integration: velocity=%f, angle=%f\n", state.velocity, state.angle);
}
//----------------------------------------------------------------------------------
int _tmain(int argc, _TCHAR* argv[])
{
	testRK4(-5, 4, 0.05f);
	fprintf_s(stdout, "\n");

	testRK4(-5, 8, 0.05f);
	fprintf_s(stdout, "\n");

	testRK4(5, 4, 0.05f);
	fprintf_s(stdout, "\n");

	testRK4(5, 8, 0.05f);
	fprintf_s(stdout, "\n");

	fprintf_s(stdout, "Press any key to exit...");
	_getch();
	


	return 0;
}

