function [] = plotTrialData(trialNum)

    % global variable declaration -----------------------------------------
    global data dataIndices idealData protocol protocolIndices dataFolderId;
    global gLevels hulkIdentifier hulkConfiguration gIntegrationStepSize;

    
    % make a color scheme -----------------------------------------------------
    colScheme = makeColorScheme_Color();

    % detect screen res, to automatically scale plots for diff displays
    ss = get(0,'ScreenSize');
    
    screenWidth = ss(3);
    screenHeight = ss(4);
 
    f1Width = 0.5 * screenWidth;
    f1Height = 0.86 * screenHeight;
    f1Left = 150;
    f1Top = 100;
 
    f2Width = 0.40 * screenWidth;
    f2Height = 0.38 * screenHeight;
    f2Left = f1Left + f1Width + 15;
    f2Top = f1Top;
    
    f3Width = 0.40 * screenWidth;
    f3Height = 0.38 * screenHeight;
    f3Left = f1Left + f1Width + 15;
    f3Top = f2Top + f2Height + 100;
        
    f1 = figure(1);
    clf;
    set(f1, 'Position', [f1Left ss(4)-f1Height-f1Top f1Width f1Height]);

    f2 = figure(2);
    clf;
    set(f2, 'Position', [f2Left ss(4)-f2Height-f2Top f2Width f2Height]);

    f3 = figure(3);
    clf;
    set(f3, 'Position', [f3Left ss(4)-f3Height-f3Top f3Width f3Height]);

    % figure out indices for trial of interest ----------------------------
    index = trialNum;
    trialData = cell2mat(data(index, 2));
    
    % find the datapoints for when the participant was actually controlling
    % the HULK
    indicesOfRelevantDatapoints = find(trialData(1:end, dataIndices.indexTrialPhase) == 2  );
    
    useTimeShift = 0;
    switch useTimeShift
        case 1
            % offset all datapoints so that t=0 is at the moment when the
            % participant started to control the HULK
            tempPositions =  trialData(indicesOfRelevantDatapoints, [dataIndices.indexTime dataIndices.indexCurrentPosRoll]);

            % repeat the last datapoint to make sure it gets included in the find
            % algorithm below
            tempPositions(end+1, 1:end) =  tempPositions(end, 1:end);

            % find index of datapoint where position actually started to increase
            tempIndices = find(tempPositions(2:end, 2) > tempPositions(1:end-1,2));
            timeAtStartOfTask = tempPositions(tempIndices(1), 1);
            indicesOfRelevantDatapoints = indicesOfRelevantDatapoints(tempIndices);

        otherwise
           timeAtStartOfTask = trialData(indicesOfRelevantDatapoints(1), dataIndices.indexTime );
    end
 
    initialVelocity = trialData(indicesOfRelevantDatapoints(1), dataIndices.indexCurrentVelRoll );
    initialAngle = trialData(indicesOfRelevantDatapoints(1), dataIndices.indexCurrentPosRoll );
 
    % calculate reference data for this trial -----------------------------
    idealData = calculateReferenceData(gLevels, initialAngle, initialVelocity, hulkConfiguration, gIntegrationStepSize);

    
    % draw plot - time vs angle -------------------------------------------
    fh = figure(1);
    clf;
    axis();
    set(gca, 'FontName', colScheme.fontFamily, 'FontSize', colScheme.fontSize, 'FontWeight', colScheme.fontWeight, 'FontUnits', 'Pixels');   
      
    set(fh, 'Name',[dataFolderId ' - ' hulkConfiguration ' - Trial #' num2str(trialNum) ' - K = ' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})]);
    title([hulkIdentifier ' - K = ' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})]);
  
    
    % plot time vs angle --------------------------------------------------
    numSteps = size(idealData,1);
    colorMap = colormap(copper(numSteps));
 
    % plot time vs angle for various g levels
    maxIdealT60 = 0;
    for index = 1:size(idealData, 1)
        hold on;
        color =  colorMap(index,:);

        g = idealData{index, 1};
        dataTime = idealData{index, 2};
        dataAngle = idealData{index, 3};
        dataVelocity = idealData{index, 4};
        indexAngle60 = idealData{index,5};
        timeAngle60 = idealData{index,6};

        plot(dataTime(1:indexAngle60), dataAngle(1:indexAngle60),'color', color, 'LineWidth', 2);
        text(dataTime(indexAngle60), dataAngle(indexAngle60),[num2str(g/9.81) 'g'], 'color', color );
        
        t60 = dataTime(indexAngle60);
        a60 = dataAngle(indexAngle60);
        if t60>maxIdealT60
            maxIdealT60 = t60;
        end
        
        plot([t60 t60], [0 a60], ':', 'color', color);
    end
    
    % plot data for trial of interest     

    % draw plot - time vs position
    plot(   trialData(indicesOfRelevantDatapoints, dataIndices.indexTime) - timeAtStartOfTask, ...
            trialData(indicesOfRelevantDatapoints, dataIndices.indexCurrentPosRoll), ...
            'color', 'r', 'LineWidth',2);
    t60 = trialData(indicesOfRelevantDatapoints(end), dataIndices.indexTime) - timeAtStartOfTask;
    a60 = trialData(indicesOfRelevantDatapoints(end), dataIndices.indexCurrentPosRoll);
    plot([t60 t60], [0 a60], ':', 'color', 'r');
    
    text(trialData(end, dataIndices.indexTime) - timeAtStartOfTask, ...
                     trialData(end, dataIndices.indexCurrentPosRoll), ...
                      [hulkIdentifier ' K=' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})], ...
                      'FontSize',8, ...
                      'color', 'r');

    minY = min(trialData(indicesOfRelevantDatapoints, dataIndices.indexCurrentPosRoll) );
    maxY = max(trialData(indicesOfRelevantDatapoints, dataIndices.indexCurrentPosRoll) );
    
    minX = min(trialData(indicesOfRelevantDatapoints, dataIndices.indexTime) - timeAtStartOfTask);
    maxX = max([maxIdealT60 max(trialData(indicesOfRelevantDatapoints, dataIndices.indexTime) - timeAtStartOfTask)]);
    plot([minX maxX], [initialAngle initialAngle], ':', 'color', [0 0 0]);
    
    disp(['plotTrialData: plotting data for trial: ' num2str(trialNum) ', min angle: ' num2str(minY) ', max angle: ' num2str(maxY) ', DOB Yaw: ' num2str(protocol{index, protocolIndices.indexDOBYaw}) ', DOB Pitch: ' num2str(protocol{index, protocolIndices.indexDOBPitch}) ', DOB Roll: ' num2str(protocol{index, protocolIndices.indexDOBRoll})]);
    xlabel('Time (s)');
    ylabel('Angle (deg)');
    
    
    % draw plot - time vs velocity ----------------------------------------
    fh = figure(2);
    axis();
    set(gca, 'FontName', colScheme.fontFamily, 'FontSize', colScheme.fontSize, 'FontWeight', colScheme.fontWeight, 'FontUnits', 'Pixels');
    hold on;
    
    set(fh, 'Name',[dataFolderId ' - ' hulkConfiguration ' - Trial #' num2str(trialNum) ' - K = ' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})]);
    title([hulkIdentifier ' - K = ' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})]);
    
    % plot time vs velocity for various g levels
    maxIdealT60=0;
    for index = 1:size(idealData, 1)
        hold on;
        color =  colorMap(index,:);

        g = idealData{index, 1};
        dataTime = idealData{index, 2};
        dataAngle = idealData{index, 3};
        dataVelocity = idealData{index, 4};
        indexAngle60 = idealData{index,5};
        timeAngle60 = idealData{index,6};

        plot(dataTime(1:indexAngle60), dataVelocity(1:indexAngle60),'color', color, 'LineWidth', 2);
        text(dataTime(indexAngle60), dataVelocity(indexAngle60),[num2str(g/9.81) 'g'], 'color', color );
        
        t60 = dataTime(indexAngle60);
        v60 = dataVelocity(indexAngle60);
        if t60>maxIdealT60
            maxIdealT60 = t60;
        end
        
        plot([t60 t60], [0 v60], ':', 'color', color);
        plot([0 t60], [v60 v60], ':', 'color', color);
    end

    plot(   trialData(indicesOfRelevantDatapoints, dataIndices.indexTime) - timeAtStartOfTask, ... 
            trialData(indicesOfRelevantDatapoints, dataIndices.indexCurrentVelRoll), ...
            'color', 'r', 'LineWidth',2);
   
    t60 = trialData(indicesOfRelevantDatapoints(end), dataIndices.indexTime) - timeAtStartOfTask;
    v60 = trialData(indicesOfRelevantDatapoints(end), dataIndices.indexCurrentVelRoll);
    plot([t60 t60], [0 v60], ':', 'color', 'r');
           
    text(trialData(end, dataIndices.indexTime) - timeAtStartOfTask, ...
                     trialData(end, dataIndices.indexCurrentVelRoll), ...
                      [hulkIdentifier ' K=' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})], ...
                      'FontSize',8, ...
                      'color', 'r');
    xlabel('Time (s)');
    ylabel('Velocity (deg/s)');

    
    % draw plot - angle vs velocity ---------------------------------------
    fh = figure(3);
    axis();
    set(gca, 'FontName', colScheme.fontFamily, 'FontSize', colScheme.fontSize, 'FontWeight', colScheme.fontWeight, 'FontUnits', 'Pixels');
    
    set(fh, 'Name',[dataFolderId ' - ' hulkConfiguration ' - Trial #' num2str(trialNum) ' - K = ' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})]);
    title([hulkIdentifier ' - K = ' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})]);
    
    % plot angle vs velocity for various g levels
    for index = 1:size(idealData, 1)
        hold on;
        color =  colorMap(index,:);

        g = idealData{index, 1};
        dataTime = idealData{index, 2};
        dataAngle = idealData{index, 3};
        dataVelocity = idealData{index, 4};
        indexAngle60 = idealData{index,5};
        timeAngle60 = idealData{index,6};

        plot(dataAngle(1:indexAngle60), dataVelocity(1:indexAngle60),'color', color, 'LineWidth', 2);
        text(dataAngle(indexAngle60), dataVelocity(indexAngle60),[num2str(g/9.81) 'g'], 'color', color );
    end
    
    plot(   trialData(indicesOfRelevantDatapoints, dataIndices.indexCurrentPosRoll), ...
            trialData(indicesOfRelevantDatapoints, dataIndices.indexCurrentVelRoll), ...
            'color', 'r', 'LineWidth',2);
        
    text(trialData(end, dataIndices.indexCurrentPosRoll), ...
                     trialData(end, dataIndices.indexCurrentVelRoll), ...
                      [hulkIdentifier ' K=' num2str(protocol{trialNum, protocolIndices.indexAccelConstant})], ...
                      'FontSize',8, ...
                      'color', 'r');
    xlabel('Angle (deg)');
    ylabel('Velocity (deg/s)');
 
end