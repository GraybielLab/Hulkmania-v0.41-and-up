clear all;
close all;
clc;

% global variable declaration ---------------------------------------------
global data dataIndices protocol protocolIndices dataFolderId ;
global gLevels hulkConfiguration hulkIdentifier gIntegrationStepSize;


% script configuration ----------------------------------------------------
dataFolderId = '20120806-2\\data';
dataFolder = [pwd '\data\' dataFolderId '\']; 
protocolName = [pwd '\data\20120806-2\tester.csv'];

g = 9.81;
% gStep = 1/10;
% gStart = 1/10;
% gEnd = 1;
% gLevels = [(gStart:gStep:gEnd)*g g/6];
gLevels = [ [0.1 1/6 0.25 0.5 0.75 1] * g];
gLevels = sort(gLevels);

gIntegrationStepSize = 0.05;    % step size for integration
hulkConfiguration = 'sitting';  % 'standing', 'sitting' or 'sitting_short'
hulkIdentifier = 'BARF';

% reference data is calculated on a trial-by-trial basis
% uncomment this if you want to calculate it only once
%initialAngle = 5;           % initial angle
%initialVelocity = 0;        % initial velocity
%idealData = calculateReferenceData(gMax, gMin, gStep, gNumIterations, initialAngle, initialVelocity, hulkConfiguration,gIntegrationStepSize);


% add folders containing helper scripts to search path --------------------
addpath( [pwd '\helpers']); 

% read data and protocol --------------------------------------------------
[data dataIndices] = readHulkamaniaData(dataFolder);
[protocol protocolIndices] = readProtocol(protocolName);

% show the ui -------------------------------------------------------------
plotTrialData(1);
showTrialInspectionUI(size(protocol,1));

