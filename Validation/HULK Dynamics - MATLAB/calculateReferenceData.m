function [ idealData ] = calculateReferenceData( gLevels, initialAngle, initialVelocity, configuration, integrationStepSize )

% compute reference data for ideal system ---------------------------------
numSteps = size(gLevels, 2);
idealData = cell(numSteps, 5);

for index = 1 : numSteps
    g = gLevels(index);
    [ dataTime dataAngle dataVelocity indexAngle60 timeAngle60 ] = inverted_pendulum_g_input(configuration, initialAngle, initialVelocity, g, integrationStepSize);

    idealData(index, 1) = {g};
    idealData(index, 2) = {dataTime};
    idealData(index, 3) = {dataAngle};
    idealData(index, 4) = {dataVelocity};
    idealData(index, 5) = {indexAngle60};
    idealData(index, 6) = {timeAngle60};
end

end

