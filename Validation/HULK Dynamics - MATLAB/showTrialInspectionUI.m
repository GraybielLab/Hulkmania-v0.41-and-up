function [] = showTrialInspectionUI(tEnd)      

global hsl;

% detect screen res, to automatically scale plots for diff displays
ss = get(0,'ScreenSize');  

frl= 10; % distance of top of main fig from top margin
frt= 30; % distance of left side of main fig from left margin

w = 120; % MainFig width
h = 100; % MainFig height

MainFig = figure('name', 'Navigation'); 
set(MainFig, 'Position', [frl ss(4)-h-frt w h], 'Menu', 'none', 'NumberTitle', 'off', 'Resize', 'off');
bc= get(MainFig, 'Color');

hFig = gcf;
hsl = sliderPanel(...
  		'Parent'  , hFig, ...
  		'Title'   , 'View trial', ...
  		'Position', [0.1 0.1 0.80 0.8], ...
        'Min'     , 1, ...
        'SliderStep', [1/(tEnd-1) 10/(tEnd-1)], ...
  		'Max'     , tEnd, ...
  		'Value'   , 1, ...
  		'FontName', 'Verdana', ...
  		'Callback', @Navigation_callback);

 uiwait(MainFig);
end

% -------------------------------------------------------------------------
function Navigation_callback(varargin)
    sldr = varargin{1};
    it = fix(get(sldr,'Value'));
    plotTrialData(it);
    hNavi = findobj('type', 'figure', 'name', 'Navigation');
    if(~isempty(hNavi))
        uistack(hNavi, 'up', 10);
    end;
end





