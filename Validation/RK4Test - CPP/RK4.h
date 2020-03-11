#ifndef _RK4_HPP
#define _RK4_HPP


		class  RungaKutta4 {
		public:
			typedef struct State {
				float angle;
				float velocity;
				State(float ang=0, float vel=0);
			} _State ;

			typedef struct Derivative {
				float dangle;
				float dvelocity;
				Derivative(float dangle=0, float dvelocity=0);
			} _Derivative;

			typedef float (*ComputeAccelerationFn)(const State &state, float t);

		private: 
			static Derivative _evaluate(const State &initial, float t, ComputeAccelerationFn fn);
			static Derivative _evaluate(const State &initial, float t, float dt, const Derivative &d, ComputeAccelerationFn fn);

		public: 
			static void integrate(State &state, float t, float dt, ComputeAccelerationFn fn);
		};

#endif