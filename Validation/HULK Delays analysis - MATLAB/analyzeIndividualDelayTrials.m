clear all;
close all;
clc;

% add folders containing helper scripts to search path --------------------
addpath( [pwd '\helpers']); 

resultsFolder = [pwd '\data\results\problemtrials\'];

% read data ---------------------------------------------------------------
[data dataIndices] = readHulkamaniaData(resultsFolder);


% plot data ---------------------------------------------------------------
outDataAmpl = [];
outDataMean = [];
outDataStd = [];

quickHack = true;
if(quickHack == true)
    warning('Applying quick hack to ensure that all reported peaks are near the global maximum and not some local maximum');
end

% figure out order in which to plot data (ascending)
allAmplitudes = zeros(size(data, 1), 1);
for index = 1:size(data, 1)
    fileName = data{index, 1};
    
    k1 = strfind(fileName, '_Amplitude') + size('_Amplitude', 2);
    k2 = strfind(fileName, '_data') - 1;
        
    allAmplitudes(index) = str2num(fileName(k1:k2));
end
[tmp sortedIndices] = sort(allAmplitudes);


% plot data
for index= 1:size(data, 1)
    
    fileName = data{index, 1};
    
    k1 = strfind(fileName, '_Amplitude') + size('_Amplitude', 2);
    k2 = strfind(fileName, '_data') - 1;
        
    amplitude = str2num(fileName(k1:k2));
    
    k = strfind(fileName, '_Hz');
    frequency = str2num(fileName(1:(k(1)-1))); 
      
    f = figure();

    subplot(2,1,1);
    hold on;
    
    ffdata = cell2mat(data(index, 2));
    
    % plot calculated (targeted) velocity
    h3 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCalcVelRoll), '-', 'color', [0 0 1]);
    
    % plot actual (reported) velocity
    h4 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCurrentVelRoll), '-', 'color', [1 0 1]);
    
    xlabel('Time (s)');
    ylabel('Velocity (deg/s)');
    title(['Frequency: ' num2str(frequency) ', Amplitude: ' num2str(amplitude)]);

    lh = legend([h3 h4], 'Calculated velocity', 'Actual velocity');
  %  set(lh, 'Location', 'EastOutside');
    
    
    subplot(2,1,2);
    hold on;
    
    dt = ffdata(2:end, dataIndices.indexTime) - ffdata(1:(end-1), dataIndices.indexTime);
    dx = ffdata(2:end, dataIndices.indexCurrentPosRoll) - ffdata(1:(end-1), dataIndices.indexCurrentPosRoll);
    dxdt = dx./dt;
    
    h3 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCurrentPosRoll), '-', 'color', [0 0 1]);
    h4 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCurrentVelRoll), '-', 'color', [1 0 1]);
    h5 = plot(ffdata(2:end, dataIndices.indexTime), dxdt, '-', 'color', [1 0 0]);
    
    xlabel('Time (s)');
%    ylabel('Velocity (deg/s)');
%    title(['Frequency: ' num2str(frequency) ', Amplitude: ' num2str(amplitude) ', ' titleStr]);

    lh = legend( 'Actual position', 'Actual velocity', 'd_p_o_s_i_t_i_o_n / dt');
 %   set(lh, 'Location', 'EastOutside');

end

disp('Done with analyzing delays!');
  



