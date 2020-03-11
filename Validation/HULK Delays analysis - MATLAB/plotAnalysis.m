function [  ] = plotAnalysis( outDataAmpl0, outDataMean0, outDataStd0, outDataXCorr0, outDataXCorrLag0, outDataAmpl30, outDataMean30, outDataStd30, outDataXCorr30, outDataXCorrLag30, titleStr )

figure;
hold on;
[tmp sIX0] = sort(outDataAmpl0);
h1 = errorbar(outDataAmpl0(sIX0), outDataMean0(sIX0), outDataStd0(sIX0), 'color', [0 0 1]);
set(gca, 'XTick', outDataAmpl0(sIX0));

[tmp sIX30] = sort(outDataAmpl30);
h2 = errorbar(outDataAmpl30(sIX30), outDataMean30(sIX30), outDataStd30(sIX30), 'color', [1 0 0]);
legend([h1 h2], 'Offset = 0', 'Offset = 30');

xlabel('Amplitude');
ylabel('Delay (s)');
title(['Delays for frequency: ' titleStr ' Hz']);

figure;
subplot(2,1,1);
hold on;
plot(outDataAmpl0(sIX0), outDataXCorr0(sIX0), 'color', [0 0 1]);
plot(outDataAmpl30(sIX30), outDataXCorr30(sIX30), 'color', [1 0 0]);
legend('Offset = 0', 'Offset = 30');
xlabel('Amplitude');
ylabel('Cross correlation');
title(['Cross correlation for frequency: ' titleStr ' Hz']);
set(gca, 'XTick', outDataAmpl0(sIX0));

subplot(2,1,2);
hold on;
plot(outDataAmpl0(sIX0), outDataXCorrLag0(sIX0), 'color', [0 0 1]);
plot(outDataAmpl30(sIX30), outDataXCorrLag30(sIX30), 'color', [1 0 0]);
legend('Offset = 0', 'Offset = 30');
xlabel('Amplitude');
ylabel('Cross correlation lag');
title(['Cross correlation lag for frequency: ' titleStr ' Hz']);
set(gca, 'XTick', outDataAmpl0(sIX0));


subplot(2,1,2);
hold on;
plot(outDataAmpl0(sIX0), outDataXCorrLag0(sIX0), 'color', [0 0 1]);
plot(outDataAmpl30(sIX30), outDataXCorrLag30(sIX30), 'color', [1 0 0]);
legend('Offset = 0', 'Offset = 30');
xlabel('Amplitude');
ylabel('Cross correlation lag');
title(['Cross correlation lag for frequency: ' titleStr ' Hz']);
set(gca, 'XTick', outDataAmpl0(sIX0));

end

