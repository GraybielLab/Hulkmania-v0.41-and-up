clear all
close all
clc
figure

for index = 1:8

    subplot(4,2,index);
    % draw samples from standard normal distribution
    x = [randn(1000,1); index-1+randn(500,1)];

    [n binCenters] = hist(x);
    h = bar(binCenters, n);

    title(['Plot ' num2str(index)]);
    disp(['Plot ' num2str(index)]);
    
    f1 = gmdistribution.fit(x,1); % single component, unimodal
    disp(['Single component AIC: ' num2str(f1.AIC)]);
    f2 = gmdistribution.fit(x,2); % two components, bimodal
    disp(['Double component AIC: ' num2str(f2.AIC)]);
    
    if(f2.AIC<f1.AIC)
        disp('Bimodal: YES');
    else
        disp('Bimodal: NO');
    end
    
    ksResult = kstest(x);
    if(ksResult == 1)
        disp('One sample Kolmogorov-Smirnoff test (.05): non-normal distribution'); 
    else
        disp('One sample Kolmogorov-Smirnoff test (.05): normal distribution'); 
    end
end

