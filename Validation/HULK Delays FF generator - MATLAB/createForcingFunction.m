function [ ff ] = createForcingFunction( frequency, duration, samplingFrequency, outFile )

ff = cell ( samplingFrequency * duration + 2, 4);
ff(1, 1:end) = {';Time' 'Yaw' 'Pitch' 'Roll'};

time = 0;
angle = 0;
dt = 1/samplingFrequency;
dangle = ( frequency / samplingFrequency) * 2 * pi;
for index = 1 : (samplingFrequency * duration + 1)
    ff(index + 1, 1) = {time};
    ff(index + 1, 2) = {0};
    ff(index + 1, 3) = {0};
    ff(index + 1, 4) = {sin(angle )};
    angle = mod(angle + dangle, 2*pi);
    time = time + dt;
end

figure;
hold on;
h1 = plot(cell2mat(ff(2:end, 1)), cell2mat(ff(2:end, 2)), 'color', [1 0 0]);
h2 = plot(cell2mat(ff(2:end, 1)), cell2mat(ff(2:end, 3)), 'color', [0 1 0]);
h3 = plot(cell2mat(ff(2:end, 1)), cell2mat(ff(2:end, 4)), 'color', [0 0 1]);
xlabel('Time');
ylabel('Deflection');
legend([h1 h2 h3], 'Yaw', 'Pitch', 'Roll');
title(outFile);

cell2csv(outFile, ff, ',');
disp(['createForcingFunction: Saved the csv file: ' outFile]);

end
