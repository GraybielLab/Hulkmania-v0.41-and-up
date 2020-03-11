function [ outDataAmpl outDataMean outDataStd outDataCrossCor outDataCrossCorLag] = plotData( data, dataIndices, titleStr, quickHack)

global frequencies;

% plot data ---------------------------------------------------------------
outDataAmpl = [];
outDataMean = [];
outDataStd = [];
outDataCrossCor = [];
outDataCrossCorLag = [];

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
f = figure();
numCols= 4;
numRows = ceil(size(data, 1)/numCols);

for sortedIndex = 1:size(data, 1)
    
    index = sortedIndices(sortedIndex);
    
    fileName = data{index, 1};
    
    k1 = strfind(fileName, '_Amplitude') + size('_Amplitude', 2);
    k2 = strfind(fileName, '_data') - 1;
        
    amplitude = str2num(fileName(k1:k2));
    
    k = strfind(fileName, '_Hz');
    frequency = str2num(fileName(1:(k(1)-1))); 
  
    frequencyIndex = find(frequencies(1:end) == frequency);
    
    subplot(numRows, numCols, sortedIndex);
    hold on;
    
    ffdata = cell2mat(data(index, 2));
    
    % plot calculated (targeted) velocity
    h3 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCalcVelRoll), '-', 'color', [0 0 1]);
    
    % plot actual (reported) velocity
    h4 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCurrentVelRoll), '-', 'color', [1 0 1]);
    
    % find peaks in calculated velocity
    [pks loc] = findpeaks(ffdata(1:end, dataIndices.indexCalcVelRoll));
    plot(ffdata(loc, dataIndices.indexTime), ffdata(loc, dataIndices.indexCalcVelRoll), 'x', 'color', [0 0 1]);
    tPeaksCalculated = ffdata(loc(2:(end-1)), dataIndices.indexTime);

    % find peaks in current velocity
    [pks loc] = findpeaks(ffdata(1:end, dataIndices.indexCurrentVelRoll));

    % quick hack to ensure that all reported peaks are near the global
    % maximum and not some local maximum
    if(quickHack == true)
        loc = loc(find(ffdata(loc, dataIndices.indexJoystickX) > 0.4));
    end
    
    plot(ffdata(loc, dataIndices.indexTime), ffdata(loc, dataIndices.indexCurrentVelRoll), 'x', 'color', [1 0 1]);
    tPeaksActual = ffdata(loc(2:(end-1)), dataIndices.indexTime);
   
    if (size(tPeaksCalculated, 1) == size(tPeaksActual, 1))
        
     %   tPeriod = tPeaksActual(2:end) - tPeaksActual(1:end-1)
        tDelta = tPeaksActual - tPeaksCalculated;
        tDeltaMean = mean(tDelta);
        tDeltaStd = std(tDelta);
        
        disp(['Amplitude: ' num2str(amplitude) ', peaks calculated: ' num2str(size(tPeaksCalculated, 1)) ', peaks actual: ' num2str(size(tPeaksActual, 1)) ', mean t delta: ' num2str(tDeltaMean) ', std t delta: ' num2str(tDeltaStd)]);
        
        dtResamp = 1/50;
        tResamp = 1:dtResamp:49;
        maxCalculated = max(abs(ffdata(1:end, dataIndices.indexCalcVelRoll)));
        tsCalculated = timeseries(ffdata(1:end, dataIndices.indexCalcVelRoll) / maxCalculated, ffdata(1:end, dataIndices.indexTime));
        tsCalculatedResamp = resample(tsCalculated, tResamp);
        
        maxActual= max(abs(ffdata(1:end, dataIndices.indexCurrentVelRoll)));
        tsActual = timeseries(ffdata(1:end, dataIndices.indexCurrentVelRoll) / maxActual, ffdata(1:end, dataIndices.indexTime) );
        tsActualResamp= resample(tsActual, tResamp);
        
%        [cc lags] = xcorr(tsCalculatedResamp.data, tsActualResamp.data);
        [cc lags] = xcorr(tsActualResamp.data, tsCalculatedResamp.data);
        maxCorrIndex = find(cc == max(cc));

%         if(amplitude == 15)
%             f2 = figure();
%             plot(lags, cc, '-', 'color', [1 0 0]);
%             xlabel('lags');
%             ylabel('cc');
%             title('Amplitude = 15');
%             
%             maxCorrIndex
%             cc(maxCorrIndex)
%             lags(maxCorrIndex)
%             lags(maxCorrIndex) * dtResamp
%             
%             figure();
%             
%             hold on;
%             
%             plot calculated (targeted) velocity
%             h31 = plot(tsCalculated.time, tsCalculated.data, '-', 'color', [0 0 1]);
%             plot calculated (targeted) velocity
%             h32 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCalcVelRoll), ':', 'color', [0 0 1]);
%     
%     plot actual (reported) velocity
%     h42 = plot(ffdata(1:end, dataIndices.indexTime), ffdata(1:end, dataIndices.indexCurrentVelRoll), ':', 'color', [1 0 1]);
%     
%             plot actual (reported) velocity
%             h41 = plot(tsActual.time, tsActual.data, '-', 'color', [1 0 1]);
%             
%             legend([h31 h41 h32 h42], 'calculated resampled', 'actual resampled', 'calculated', 'actual');
%             
%             figure(f);
%         end
        
        outDataAmpl(end+1) = amplitude;
        outDataMean(end+1) = tDeltaMean;
        outDataStd(end+1) = tDeltaStd;
        outDataCrossCor(end+1) = cc(maxCorrIndex);
        outDataCrossCorLag(end+1) = lags(maxCorrIndex) * dtResamp;
        
    else
        disp(['Problem with amplitude: ' num2str(amplitude) ', peaks calculated: ' num2str(size(tPeaksCalculated, 1)) ', peaks actual: ' num2str(size(tPeaksActual, 1))]);
    end
     
    xlabel('Time (s)');
    ylabel('Velocity (deg/s)');
    title(['Frequency: ' num2str(frequency) ', Amplitude: ' num2str(amplitude) ', ' titleStr]);

    if(sortedIndex == size(data, 1))
        lh = legend([h3 h4], 'Calculated velocity', 'Actual velocity');
%        lh = legend([h1 h3 h4], 'Forcing function * amplitude', 'Calculated velocity', 'Actual velocity');
        set(lh, 'Location', 'EastOutside');
    end
    
end

end

