function [ dataTime dataAngle dataVelocity indexAngle60 timeAngle60 ] = inverted_pendulum_g_input( simtype, initialAngle, initialVelocity, gravityConstant, integrationStep )

% This script runs the inverted pendulum simulation for "falling" human subjects
% The acceleration of gravity is input as a parameter
% 
% Standing = human subject standing, rotating around the ankle
% Sitting  = human subject sitting, rotating around an axis aligned with CG, at foot level
% Sitting (short rotation arm) = Sitting  = human subject sitting, rotating around an axis aligned with CG, just under the seat
%
% Anthropometric paramenters are taken from C.Clauser 1963 (Air Force Technical Report NO. AMRL-TDR-63-36)


% run simulation or load saved simulation ---------------------------------
%open('simmechanics_inverted_pendulum_v5');
%load('inverted_pendulum_data') % NOTE: This dataset was created with a small (0.001s) integration step.

IC = initialAngle;
IV = initialVelocity;
g = gravityConstant;

% compute results ---------------------------------------------------------

step_v = integrationStep; % This is the size [s] of the simulation integration step. Change to higher value for faster, but less precise, integration  
options=simset('dstworkspace','current', 'srcworkspace', 'current');
sim('simmechanics_inverted_pendulum_v6', [] , options);

% The joint angle data is stored in the workspace in structure Joint_angle
time            = Joint_angle.time;

standing_a      = Joint_angle.signals(1,1).values(:,1);
sitting_a       = Joint_angle.signals(1,2).values(:,1);
sitting_short_a = Joint_angle.signals(1,3).values(:,1);

standing_a_vel      = Joint_angle.signals(1,1).values(:,2);
sitting_a_vel       = Joint_angle.signals(1,2).values(:,2);
sitting_short_a_vel = Joint_angle.signals(1,3).values(:,2);

% Find the instant at which the angle of 60 degrees is reached by the
% falling bodies
% NOTE: function "crossing" is not a standard matlab function, it can be
% downloaded here: http://www.mathworks.com/matlabcentral/fileexchange/2432

[ind_standing,t60_standing]           = crossing(standing_a,time,60);
[ind_sitting,t60_sitting]             = crossing(sitting_a,time,60);
[ind_sitting_short,t60_sitting_short] = crossing(sitting_short_a,time,60);

ind_standing     = ind_standing(1);
ind_sitting      = ind_sitting(1);
ind_sitting_short= ind_sitting_short(1);

% these are the times when angle is 60 degrees 
t60_standing      = t60_standing(1);
t60_sitting       = t60_sitting(1);
t60_sitting_short = t60_sitting_short(1);


% detemine what data the caller is interested in --------------------------
st = 0; % default: standing

if(strcmpi(simtype, 'sitting') == 1)
st = 1; % sitting
end

if(strcmpi(simtype, 'sitting_short') == 1)
st = 2; % sitting_short
end

disp(['inverted_pendulum_g_input: calculating for g:' num2str(g) ', initial angle: ' num2str(IC) ', initial velocity: ' num2str(IV) ', integration step size: ' num2str(integrationStep) ', requested data type ' simtype ' = ' num2str(st) ]);

switch st
    case 0
        dataTime = time;
        dataAngle = standing_a;
        dataVelocity = standing_a_vel;
        indexAngle60 = ind_standing;
        timeAngle60 = t60_standing;
    case 1
        dataTime = time;
        dataAngle = sitting_a;
        dataVelocity = sitting_a_vel;
        indexAngle60 = ind_sitting;
        timeAngle60 = t60_sitting;
    case 2
        dataTime = time;
        dataAngle = sitting_short_a;
        dataVelocity = sitting_short_a_vel;
        indexAngle60 = ind_sitting_short;
        timeAngle60 = t60_sitting_short;
end

end
