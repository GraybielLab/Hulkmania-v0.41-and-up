clear all;
close all;
clc;

% add folders containing helper scripts to search path --------------------
addpath( [pwd '\helpers']); 

% forcing function generation parameters ----------------------------------
frequencies = [0.1 0.2 0.4 0.8 1.0 1.6 2.5 4.5 5 6.4];  % Hz
baseOutFolder = [pwd '\Output\'];
duration = 50;                                  % seconds
samplingRate = 50;                              % Hz

% generate all the forcing function files ---------------------------------
for index = 1:size(frequencies, 2)
    frequency = frequencies(index);
    outFile = [baseOutFolder num2str(frequency) '_Hz_Roll_' num2str(duration) '_sec.csv'];
    createForcingFunction(frequency, duration, samplingRate, outFile);
end

disp('Done with forcing function creation.');