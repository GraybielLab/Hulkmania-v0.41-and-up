function [ protocol propertyIndices ] = readForcingFunction( fileName )
    propertyIndices.indexTime = 1;
    propertyIndices.indexYaw = 2;
    propertyIndices.indexPitch = 3;
    propertyIndices.indexRoll = 4;

    [protocol, result] = readtext( fileName, ',', ';', '"');
end

