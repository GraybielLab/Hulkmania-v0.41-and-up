function [ cols ] = makeColorScheme_Color(  )

% http://colorschemedesigner.com
% http://www.colorblender.com/
% http://www.colourlovers.com

cols = {};

% -------------------------------------------------------------------------
cols.referenceLineWidth = 2.0;
cols.referenceLineStyle = ':';
cols.referenceLineColor = [000/255 000/255 000/255];

cols.axisTickSizeX = 0.025;
cols.axisTickSizeY = 0.01;

% used in all banded plots (instead of plotting mean/std, plot std as color
% band)
cols.transparencyFace = 0.25;
cols.transparencyEdge = 0.60;

cols.fontSize = 10;
cols.fontWeight = 'Bold';
cols.fontFamily = 'Arial';

% -------------------------------------------------------------------------
% all dual histogram plots
% -------------------------------------------------------------------------
cols.attainedAngleColorFace = [180/255 244/255 084/255];
cols.attainedAngleColorEdge = [045/255 061/255 021/255];
cols.intendedAngleColorFace = [084/255 180/255 244/255];
cols.intendedAngleColorEdge = [021/255 045/255 122/255];

% used to indicate mean and std in dual histogram plots
cols.meanColor  = [000/255 000/255 000/255];
cols.stdColor   = [000/255 000/255 000/255];

% reference line that indicates set DOB
cols.dobColor = [255/255 000/255 000/255];
cols.dobLineStyle = '--';
cols.dobLineWidth = 2.0;

% reference line that indicates gravitational upright
cols.gravitationalUprightColor = [000/255 000/255 000/255];
cols.gravitationalUprightLineStyle = '--';
cols.gravitationalUprightLineWidth = 2.0;

cols.histogramTopMargin = 25;   % increases the max histogram frequency by this percentage
                                % and uses that as a range for the axes

% -------------------------------------------------------------------------
% createGeneric_TrialDataFigure
% -------------------------------------------------------------------------
% the following properties are used to indicate joystick button presses 
% in trial plot
cols.intendedAngleMarkerColor = [1 0 0];   
cols.intendedAngleMarkerLineWidth = 2;
cols.intendedAngleMarkerSize = 20;
cols.intendedAngleMarkerStyle = '.';

cols.lineWidth = 2;

% color for various graphs
cols.angleColor = [0 0 0];          % roll angle
cols.joystickColor = [0 0 0];       % intended angle
cols.velocityColor = [0 0 0];       % angular velocity
cols.psdColor = [0 0 0];            % power spectral density

% the following properties are used to indicate trial phases
cols.trialPhaseColor = zeros(3,3);
cols.trialPhaseColor(1, :) = [220/255 250/255 250/255]; % moving DOB
cols.trialPhaseColor(2, :) = [250/255 250/255 220/255]; % task active
cols.trialPhaseColor(3, :) = [250/255 220/255 250/255]; % repositioning

% -------------------------------------------------------------------------
% createControl2_DataOverviewFigure.m
% -------------------------------------------------------------------------
cols.instructionTypeCols = zeros(4,3);
cols.instructionTypeCols(1, :) = [229/255 100/255 078/255];
cols.instructionTypeCols(2, :) = [069/255 211/255 173/255];
cols.instructionTypeCols(3, :) = [237/255 223/255 078/255];
cols.instructionTypeCols(4, :) = [051/255 153/255 255/255];

% -------------------------------------------------------------------------
% plotRegression.m
% -------------------------------------------------------------------------
%cols.regressionPlotMarkers = ['d', 's', 'o', '*'];
cols.regressionPlotMarkers = [' ', ' ', ' ', ' '];
cols.regressionPlotLineStyleNS = '--';
cols.regressionPlotLineStyleS = '-';
% cols.regressionPlotCols = zeros(4,3);
% cols.regressionPlotCols(1, :) = 0/255;
% cols.regressionPlotCols(2, :) = 0/255;
% cols.regressionPlotCols(3, :) = 0/255;
% cols.regressionPlotCols(4, :) = 0/255;
cols.regressionPlotCols = cols.instructionTypeCols;

% -------------------------------------------------------------------------
% plotErrorBars.m
% -------------------------------------------------------------------------
cols.errorBarMarkerSize = 6;
% cols.errorBarCols = zeros(4,3);
% cols.errorBarCols(1, :) = 172/255;
% cols.errorBarCols(2, :) = 172/255;
% cols.errorBarCols(3, :) = 172/255;
% cols.errorBarCols(4, :) = 172/255;
cols.errorBarCols = cols.instructionTypeCols;

% -------------------------------------------------------------------------
% plotBarSeries.m
% -------------------------------------------------------------------------
% cols.barSeriesCols = zeros(4,3);
% cols.barSeriesCols(1, :) = 112/255;
% cols.barSeriesCols(2, :) = 152/255;
% cols.barSeriesCols(3, :) = 192/255;
% cols.barSeriesCols(4, :) = 232/255;
cols.barSeriesCols = cols.instructionTypeCols;

end

