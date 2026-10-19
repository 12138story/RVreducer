#include "TANBearing.h"

//	bearing1.fh_switch(L);
//	bearing1.fn_switch(n);
//bearing1.Para_switch(d, D);

double NSBearingStaticArrary::sLhListInBook[210] = { 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500, 520, 540, 560, 580, 600, 620, 640, 660, 680, 700, 720, 740, 760, 780, 800, 820, 840, 860, 880, 900, 920, 940, 960, 980, 1000, 1050, 1100, 1150, 1200, 1250, 1300, 1350, 1400, 1450, 1500, 1550, 1600, 1650, 1700, 1750, 1800, 1850, 1900, 1950, 2000, 2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000, 3100, 3200, 3300, 3400, 3500, 3600, 3700, 3800, 3900, 4000, 4100, 4200, 4300, 4400, 4500, 4600, 4700, 4800, 4900, 5000, 5200, 5400, 5600, 5800, 6000, 6200, 6400, 6600, 6800, 7000, 7200, 7400, 7600, 7800, 8000, 8200, 8400, 8600, 8800, 9000, 9200, 9400, 9600, 9800, 10000, 10500, 11000, 11500, 12000, 12500, 13000, 13500, 14000, 14500, 15000, 15500, 16000, 16500, 17000, 17500, 18000, 18500, 19000, 19500, 20000, 21000, 22000, 23000, 24000, 25000, 26000, 27000, 28000, 29000, 30000, 31000, 32000, 33000, 34000, 35000, 36000, 37000, 38000, 39000, 40000, 41000, 42000, 43000, 44000, 45000, 46000, 47000, 48000, 49000, 50000, 55000, 60000, 65000, 70000, 75000, 80000, 85000, 90000, 100000 };
double NSBearingStaticArrary::sFhListInBook[210] = { 0.617,0.626,0.635,0.643,0.652,0.66,0.668,0.675,0.683,0.69,0.697,0.704,0.71,0.717,0.723,0.73,0.736,0.724,0.748,0.754,0.76,0.771,0.782,0.792,0.802,0.812,0.822,0.831,0.84,0.849,0.858,0.866,0.875,0.883,0.891,0.898,0.906,0.914,0.921,0.928,0.935,0.942,0.949,0.956,0.962,0.969,0.975,0.982,0.988,0.994,1,1.01,1.025,1.035,1.045,1.055,1.065,1.075,1.085,1.095,1.105,1.115,1.125,1.135,1.145,1.151,1.16,1.17,1.18,1.185,1.19,1.2,1.21,1.215,1.225,1.23,1.25,1.27,1.285,1.3,1.315,1.33,1.345,1.36,1.375,1.39,1.405,1.42,1.43,1.445,1.455,1.47,1.48,1.49,1.505,1.515,1.54,1.56,1.58,1.6,1.62,1.64,1.66,1.675,1.695,1.71,1.73,1.745,1.76,1.775,1.795,1.81,1.825,1.84,1.85,1.865,1.88,1.895,1.905,1.92,1.935,1.945,1.96,1.97,1.985,2,2.02,2.04,2.06,2.09,2.11,2.13,2.15,2.17,2.19,2.21,2.23,2.24,2.26,2.28,2.3,2.31,2.33,2.35,2.36,2.38,2.4,2.41,2.43,2.44,2.46,2.49,2.53,2.56,2.59,2.63,2.66,2.69,2.72,2.75,2.77,2.8,2.83,2.85,2.88,2.91,2.93,2.95,2.98,3,3.02,3.07,3.11,3.15,3.19,3.23,3.27,3.31,3.35,3.38,3.42,3.45,3.48,3.51,3.55,3.58,3.61,3.64,3.67,3.7,3.72,3.75,3.78,3.8,3.83,3.86,3.88,3.91,3.93,3.96,3.98,4.1,4.2,4.3,4.4,4.5,4.58,4.68,4.75,4.9 };
double NSBearingStaticArrary::sFh2LhCoef[420] = { 100,581.008,105,536.591,110,605.963,115,590.319,120,585.476,125,619.953,130,684.711,135,673.358,140,655.634,145,735.668,150,687.41,155,800.408,160,774.583,165,769.187,170,815.827,175,626.888,180,1309.01,185,-2378.2,190,986.637,195,784.207,200,876.535,210,905.861,220,954.567,230,1011.12,240,1000.95,250,985.065,260,1058.79,270,1123.38,280,1114.34,290,1085.92,300,1208.64,310,1181.71,320,1170.08,330,1242.26,340,1360.87,350,1376.71,360,1192.49,370,1353.34,380,1448.51,390,1424.05,400,1426.72,410,1440.5,420,1382.72,430,1600.07,440,1545.18,450,1556.52,460,1567.67,470,1509.86,480,1721.69,490,1603.36,500,1864.86,520,1715.14,540,1626.99,560,2099.95,580,1973.22,600,2007.17,620,1998.1,640,2000.42,660,2000.22,680,1998.7,700,2005,720,1981.31,740,2069.76,760,1739.67,780,2971.57,800,3047.17,820,1973.48,840,1689.2,860,3269.71,880,4346.26,900,3345.26,920,1235.92,940,3711.07,960,3248.83,980,3084.89,1000,4120.91,1050,1951.29,1100,3073.91,1150,3402.84,1200,3314.74,1250,3338.2,1300,3332.48,1350,3331.9,1400,3339.92,1450,3308.43,1500,3426.36,1550,2986.14,1600,4629.1,1650,4245.59,1700,4328.41,1750,4408.23,1800,3966.22,1850,5507.12,1900,4005.31,1950,4212.79,2000,4953.82,2100,4291.29,2200,5188.29,2300,4955.54,2400,4989.54,2500,5086.31,2600,4665.21,2700,6252.84,2800,5866.15,2900,5954.19,3000,6010.72,3100,5677.74,3200,6869.88,3300,6842.76,3400,5759.09,3500,5667.22,3600,7095.43,3700,5951.08,3800,9100.25,3900,9031.77,4000,6190.77,4100,6205.15,4200,8988.64,4300,9234.43,4400,5344.9,4500,9385.98,4600,8483.48,4700,8503.63,4800,9332.25,4900,5583.3,5000,8334.57,5200,10327.6,5400,10354.9,5600,8252.66,5800,8204.32,6000,10483.8,6200,9860.36,6400,10074.7,6600,9840.78,6800,10562.2,7000,7910.59,7200,17795.5,7400,17658.2,7600,8459.69,7800,8503.03,8000,17528.2,8200,18164,8400,5959.92,8600,17996.4,8800,18031,9000,5821.54,9200,18682.9,9400,16040.6,9600,16390.5,9800,17808.1,10000,10370.2,10500,16436.6,11000,13635.5,11500,18073.2,12000,14071.8,12500,14400.8,13000,17168.2,13500,16926.2,14000,15127,14500,22565.9,15000,23029,15500,13505.9,16000,22947.2,16500,22838.6,17000,13886,17500,21617.4,18000,27018,18500,20310.6,19000,20420.1,19500,26726.1,20000,22675.5,21000,21956.1,22000,25817.7,23000,24773,24000,25090.4,25000,24865.5,26000,25447.5,27000,23344.5,28000,31174.4,29000,29631.4,30000,28487.8,31000,34319,32000,34236.1,33000,28736.6,34000,28581.3,35000,34663.2,36000,32766,37000,34272.9,38000,30142.4,39000,45157.7,40000,46046.3,41000,27032.3,42000,45824.7,43000,45896.1,44000,26782.5,45000,46974,46000,42231.8,47000,43380,48000,43912,49000,40370,50000,52825.2,55000,43227,60000,51646.5,65000,50186.9,70000,47606.1,75000,59388.9,80000,55615,85000,59871.2,90000,76793.1,100000,48188.8 };
double NSBearingStaticArrary::sLh2FhCoef[420] = { 0.617,0.00173333,0.626,0.00185136,0.635,0.00166124,0.643,0.00170367,0.652,0.00172407,0.66,0.00160003,0.668,0.00147581,0.675,0.00149675,0.683,0.00153721,0.69,0.00135441,0.697,0.00144515,0.704,0.00126499,0.71,0.00129488,0.717,0.00135548,0.723,0.00108319,0.73,0.00211176,0.736,-0.00173023,0.724,0.00120915,0.748,0.00409363,0.754,0.000416334,0.76,0.00144104,0.771,0.00102112,0.782,0.00107448,0.792,0.000980941,0.802,0.00100175,0.812,0.00101205,0.822,0.000950038,0.831,0.000887794,0.84,0.000898785,0.849,0.000917066,0.858,0.000832951,0.866,0.000851132,0.875,0.000862523,0.883,0.000798775,0.891,0.000742376,0.898,0.000731722,0.906,0.000830737,0.914,0.00074533,0.921,0.000687941,0.928,0.000702905,0.935,0.000700439,0.942,0.000695337,0.949,0.000718212,0.956,0.000631816,0.962,0.000654524,0.969,0.000650087,0.975,0.000645128,0.982,0.000669401,0.988,0.000577267,0.994,0.000621532,1,0.000536605,1.01,0.000637306,1.025,0.000664171,1.035,0.000456011,1.045,0.000511787,1.055,0.000496843,1.065,0.000500843,1.075,0.000499785,1.085,0.000500017,1.095,0.000500148,1.105,0.000499391,1.115,0.000502286,1.125,0.000491463,1.135,0.00053186,1.145,0.000381096,1.151,0.000343755,1.16,0.000493883,1.17,0.000530711,1.18,0.000383273,1.185,0.000186199,1.19,0.000371933,1.2,0.00057607,1.21,0.000323786,1.215,0.000378786,1.225,0.000411071,1.23,0.00022693,1.25,0.000458809,1.27,0.000337832,1.285,0.000289863,1.3,0.000302714,1.315,0.000299279,1.33,0.00030017,1.345,0.000300042,1.36,0.000299663,1.375,0.000301308,1.39,0.000295106,1.405,0.000318269,1.42,0.000231817,1.43,0.000254463,1.445,0.000250333,1.455,0.000244207,1.47,0.000272839,1.48,0.000164437,1.49,0.000269414,1.505,0.000257908,1.515,0.000198954,1.54,0.000240461,1.56,0.000189202,1.58,0.00020273,1.6,0.000199878,1.62,0.00019776,1.64,0.000209083,1.66,0.000165908,1.675,0.000177284,1.695,0.000174958,1.71,0.000172886,1.73,0.000183497,1.745,0.000143126,1.76,0.000143999,1.775,0.000180878,1.795,0.000182487,1.81,0.000139172,1.825,0.000160825,1.84,0.000117528,1.85,0.000119063,1.865,0.000156221,1.88,0.000156055,1.895,0.000119561,1.905,0.000115701,1.92,0.000167634,1.935,0.000113763,1.945,0.000127316,1.96,0.000126975,1.97,0.000114785,1.985,0.000163884,2,0.000129679,2.02,9.416e-05,2.04,9.36812e-05,2.06,0.000131115,2.09,0.000131858,2.11,9.14527e-05,2.13,0.000102331,2.15,9.9223e-05,2.17,0.000100777,2.19,9.76686e-05,2.21,0.000108548,2.23,6.81378e-05,2.24,6.89004e-05,2.26,0.00010626,2.28,0.000106058,2.3,6.95083e-05,2.31,6.5909e-05,2.33,0.000116856,2.35,6.66682e-05,2.36,6.64715e-05,2.38,0.000117446,2.4,6.37449e-05,2.41,7.75745e-05,2.43,7.59572e-05,2.44,6.85967e-05,2.46,9.96559e-05,2.49,6.09166e-05,2.53,7.66775e-05,2.56,5.23735e-05,2.59,7.38286e-05,2.63,7.23121e-05,2.66,5.69231e-05,2.69,5.99955e-05,2.72,6.30949e-05,2.75,4.76249e-05,2.77,4.64056e-05,2.8,6.67526e-05,2.83,4.65839e-05,2.85,4.69117e-05,2.88,6.57693e-05,2.91,5.00113e-05,2.93,3.41856e-05,2.95,5.32463e-05,2.98,5.28292e-05,3,3.54368e-05,3.02,4.54234e-05,3.07,4.65859e-05,3.11,3.8233e-05,3.15,4.04821e-05,3.19,3.98387e-05,3.23,4.0163e-05,3.27,3.95093e-05,3.31,4.17997e-05,3.35,3.32919e-05,3.38,3.50328e-05,3.42,3.65768e-05,3.45,2.86601e-05,3.48,2.87828e-05,3.51,3.62086e-05,3.55,3.63829e-05,3.58,2.826e-05,3.61,3.05773e-05,3.64,2.94308e-05,3.67,3.16996e-05,3.7,2.37709e-05,3.72,2.32169e-05,3.75,3.33617e-05,3.78,2.33365e-05,3.8,2.32925e-05,3.83,3.34935e-05,3.86,2.27334e-05,3.88,2.55729e-05,3.91,2.49751e-05,3.93,2.45267e-05,3.96,2.69179e-05,3.98,1.78016e-05,4.1,2.37914e-05,4.2,1.90329e-05,4.3,2.0077e-05,4.4,2.06592e-05,4.5,1.72863e-05,4.58,1.81955e-05,4.68,1.79315e-05,4.75,1.20784e-05,4.9,2.06667e-05 };
CBearingTAN::CBearingTAN()
{
	InitialLifeCurve();
}
CBearingTAN::CBearingTAN(double n, int L, int d, int D) : n{ n }, L{ L }, d{ d }, D{ D }
{
	Para_switch(d, D);
	InitialLifeCurve();
}

CBearingTAN::CBearingTAN(int InBearingType, double* InBearingData)
{
	DefineRollingBearing(InBearingType, InBearingData);
	InitialLifeCurve();
}

CBearingTAN::CBearingTAN(double* InBearingData)
{
	DefineRollingBearing(int(InBearingData[0]), InBearingData + 1);
	InitialLifeCurve();
}

CBearingTAN::~CBearingTAN()
{

}


double CBearingTAN::fhSet(int L1)
{
#pragma region fh
	int L{ 0 };
	if (L1 >= 100 && L1 <= 200) L = (L1 / 5) * 5;
	if (L1 > 200 && L1 <= 500) L = (L1 / 10) * 10;
	if (L1 > 500 && L1 <= 1000) L = (L1 / 20) * 20;
	if (L1 > 1000 && L1 <= 2000) L = (L1 / 50) * 50;
	if (L1 > 2000 && L1 <= 5000) L = (L1 / 100) * 100;
	if (L1 > 5000 && L1 <= 10000) L = (L1 / 200) * 200;
	if (L1 > 10000 && L1 <= 20000) L = (L1 / 500) * 500;
	if (L1 > 20000 && L1 <= 50000) L = (L1 / 1000) * 1000;
	if (L1 > 50000 && L1 <= 100000) L = (L1 / 5000) * 5000;
	switch (L)  //查表求fh值
	{
#pragma region L=100
	case 100:
		fh = 0.617;
		break;
#pragma endregion
#pragma region L=105
	case 105:
		fh = 0.626;
		break;
#pragma endregion
#pragma region L=110
	case 110:
		fh = 0.635;
		break;
#pragma endregion
#pragma region L=115
	case 115:
		fh = 0.643;
		break;
#pragma endregion
#pragma region L=120
	case 120:
		fh = 0.652;
		break;
#pragma endregion
#pragma region L=125
	case 125:
		fh = 0.660;
		break;
#pragma endregion
#pragma region L=130
	case 130:
		fh = 0.668;
		break;
#pragma endregion
#pragma region L=135
	case 135:
		fh = 0.675;
		break;
#pragma endregion
#pragma region L=140
	case 140:
		fh = 0.683;
		break;
#pragma endregion
#pragma region L=145
	case 145:
		fh = 0.690;
		break;
#pragma endregion
#pragma region L=150
	case 150:
		fh = 0.697;
		break;
#pragma endregion
#pragma region L=155
	case 155:
		fh = 0.704;
		break;
#pragma endregion
#pragma region L=160
	case 160:
		fh = 0.710;
		break;
#pragma endregion
#pragma region L=165
	case 165:
		fh = 0.717;
		break;
#pragma endregion
#pragma region L=170
	case 170:
		fh = 0.723;
		break;
#pragma endregion
#pragma region L=175
	case 175:
		fh = 0.730;
		break;
#pragma endregion
#pragma region L=180
	case 180:
		fh = 0.736;
		break;
#pragma endregion
#pragma region L=185
	case 185:
		fh = 0.724;
		break;
#pragma endregion
#pragma region L=190
	case 190:
		fh = 0.748;
		break;
#pragma endregion
#pragma region L=195
	case 195:
		fh = 0.754;
		break;
#pragma endregion
#pragma region L=200
	case 200:
		fh = 0.760;
		break;
#pragma endregion
#pragma region L=210
	case 210:
		fh = 0.771;
		break;
#pragma endregion
#pragma region L=220
	case 220:
		fh = 0.782;
		break;
#pragma endregion
#pragma region L=230
	case 230:
		fh = 0.792;
		break;
#pragma endregion
#pragma region L=240
	case 240:
		fh = 0.802;
		break;
#pragma endregion
#pragma region L=250
	case 250:
		fh = 0.812;
		break;
#pragma endregion
#pragma region L=260
	case 260:
		fh = 0.822;
		break;
#pragma endregion
#pragma region L=270
	case 270:
		fh = 0.831;
		break;
#pragma endregion
#pragma region L=280
	case 280:
		fh = 0.840;
		break;
#pragma endregion
#pragma region L=290
	case 290:
		fh = 0.849;
		break;
#pragma endregion
#pragma region L=300
	case 300:
		fh = 0.858;
		break;
#pragma endregion
#pragma region L=310
	case 310:
		fh = 0.866;
		break;
#pragma endregion
#pragma region L=320
	case 320:
		fh = 0.875;
		break;
#pragma endregion
#pragma region L=330
	case 330:
		fh = 0.883;
		break;
#pragma endregion
#pragma region L=340
	case 340:
		fh = 0.891;
		break;
#pragma endregion
#pragma region L=350
	case 350:
		fh = 0.898;
		break;
#pragma endregion
#pragma region L=360
	case 360:
		fh = 0.906;
		break;
#pragma endregion
#pragma region L=370
	case 370:
		fh = 0.914;
		break;
#pragma endregion
#pragma region L=380
	case 380:
		fh = 0.921;
		break;
#pragma endregion
#pragma region L=390
	case 390:
		fh = 0.928;
		break;
#pragma endregion
#pragma region L=400
	case 400:
		fh = 0.935;
		break;
#pragma endregion
#pragma region L=410
	case 410:
		fh = 0.942;
		break;
#pragma endregion
#pragma region L=420
	case 420:
		fh = 0.949;
		break;
#pragma endregion
#pragma region L=430
	case 430:
		fh = 0.956;
		break;
#pragma endregion
#pragma region L=440
	case 440:
		fh = 0.962;
		break;
#pragma endregion
#pragma region L=450
	case 450:
		fh = 0.969;
		break;
#pragma endregion
#pragma region L=460
	case 460:
		fh = 0.975;
		break;
#pragma endregion
#pragma region L=470
	case 470:
		fh = 0.982;
		break;
#pragma endregion
#pragma region L=480
	case 480:
		fh = 0.988;
		break;
#pragma endregion
#pragma region L=490
	case 490:
		fh = 0.994;
		break;
#pragma endregion
#pragma region L=500
	case 500:
		fh = 1.000;
		break;
#pragma endregion
#pragma region L=520
	case 520:
		fh = 1.010;
		break;
#pragma endregion
#pragma region L=540
	case 540:
		fh = 1.025;
		break;
#pragma endregion
#pragma region L=560
	case 560:
		fh = 1.035;
		break;
#pragma endregion
#pragma region L=580
	case 580:
		fh = 1.045;
		break;
#pragma endregion
#pragma region L=600
	case 600:
		fh = 1.055;
		break;
#pragma endregion
#pragma region L=620
	case 620:
		fh = 1.065;
		break;
#pragma endregion
#pragma region L=640
	case 640:
		fh = 1.075;
		break;
#pragma endregion
#pragma region L=660
	case 660:
		fh = 1.085;
		break;
#pragma endregion
#pragma region L=680
	case 680:
		fh = 1.095;
		break;
#pragma endregion
#pragma region L=700
	case 700:
		fh = 1.105;
		break;
#pragma endregion
#pragma region L=720
	case 720:
		fh = 1.115;
		break;
#pragma endregion
#pragma region L=740
	case 740:
		fh = 1.125;
		break;
#pragma endregion
#pragma region L=760
	case 760:
		fh = 1.135;
		break;
#pragma endregion
#pragma region L=780
	case 780:
		fh = 1.145;
		break;
#pragma endregion
#pragma region L=800
	case 800:
		fh = 1.151;
		break;
#pragma endregion
#pragma region L=820
	case 820:
		fh = 1.160;
		break;
#pragma endregion
#pragma region L=840
	case 840:
		fh = 1.170;
		break;
#pragma endregion
#pragma region L=860
	case 860:
		fh = 1.180;
		break;
#pragma endregion
#pragma region L=880
	case 880:
		fh = 1.185;
		break;
#pragma endregion
#pragma region L=900
	case 900:
		fh = 1.190;
		break;
#pragma endregion
#pragma region L=920
	case 920:
		fh = 1.200;
		break;
#pragma endregion
#pragma region L=940
	case 940:
		fh = 1.210;
		break;
#pragma endregion
#pragma region L=960
	case 960:
		fh = 1.215;
		break;
#pragma endregion
#pragma region L=980
	case 980:
		fh = 1.225;
		break;
#pragma endregion
#pragma region L=1000
	case 1000:
		fh = 1.230;
		break;
#pragma endregion
#pragma region L=1050
	case 1050:
		fh = 1.250;
		break;
#pragma endregion
#pragma region L=1100
	case 1100:
		fh = 1.270;
		break;
#pragma endregion
#pragma region L=1150
	case 1150:
		fh = 1.285;
		break;
#pragma endregion
#pragma region L=1200
	case 1200:
		fh = 1.300;
		break;
#pragma endregion
#pragma region L=1250
	case 1250:
		fh = 1.315;
		break;
#pragma endregion
#pragma region L=1300
	case 1300:
		fh = 1.330;
		break;
#pragma endregion
#pragma region L=1350
	case 1350:
		fh = 1.345;
		break;
#pragma endregion
#pragma region L=1400
	case 1400:
		fh = 1.360;
		break;
#pragma endregion
#pragma region L=1450
	case 1450:
		fh = 1.375;
		break;
#pragma endregion
#pragma region L=1500
	case 1500:
		fh = 1.390;
		break;
#pragma endregion
#pragma region L=1550
	case 1550:
		fh = 1.405;
		break;
#pragma endregion
#pragma region L=1600
	case 1600:
		fh = 1.420;
		break;
#pragma endregion
#pragma region L=1650
	case 1650:
		fh = 1.430;
		break;
#pragma endregion
#pragma region L=1700
	case 1700:
		fh = 1.445;
		break;
#pragma endregion
#pragma region L=1750
	case 1750:
		fh = 1.455;
		break;
#pragma endregion
#pragma region L=1800
	case 1800:
		fh = 1.470;
		break;
#pragma endregion
#pragma region L=1850
	case 1850:
		fh = 1.480;
		break;
#pragma endregion
#pragma region L=1900
	case 1900:
		fh = 1.490;
		break;
#pragma endregion
#pragma region L=1950
	case 1950:
		fh = 1.505;
		break;
#pragma endregion
#pragma region L=2000
	case 2000:
		fh = 1.515;
		break;
#pragma endregion
#pragma region L=2100
	case 2100:
		fh = 1.540;
		break;
#pragma endregion
#pragma region L=2200
	case 2200:
		fh = 1.560;
		break;
#pragma endregion
#pragma region L=2300
	case 2300:
		fh = 1.580;
		break;
#pragma endregion
#pragma region L=2400
	case 2400:
		fh = 1.600;
		break;
#pragma endregion
#pragma region L=2500
	case 2500:
		fh = 1.620;
		break;
#pragma endregion
#pragma region L=2600
	case 2600:
		fh = 1.640;
		break;
#pragma endregion
#pragma region L=2700
	case 2700:
		fh = 1.660;
		break;
#pragma endregion
#pragma region L=2800
	case 2800:
		fh = 1.675;
		break;
#pragma endregion
#pragma region L=2900
	case 2900:
		fh = 1.695;
		break;
#pragma endregion
#pragma region L=3000
	case 3000:
		fh = 1.710;
		break;
#pragma endregion
#pragma region L=3100
	case 3100:
		fh = 1.730;
		break;
#pragma endregion
#pragma region L=3200
	case 3200:
		fh = 1.745;
		break;
#pragma endregion
#pragma region L=3300
	case 3300:
		fh = 1.760;
		break;
#pragma endregion
#pragma region L=3400
	case 3400:
		fh = 1.775;
		break;
#pragma endregion
#pragma region L=3500
	case 3500:
		fh = 1.795;
		break;
#pragma endregion
#pragma region L=3600
	case 3600:
		fh = 1.810;
		break;
#pragma endregion
#pragma region L=3700
	case 3700:
		fh = 1.825;
		break;
#pragma endregion
#pragma region L=3800
	case 3800:
		fh = 1.840;
		break;
#pragma endregion
#pragma region L=3900
	case 3900:
		fh = 1.850;
		break;
#pragma endregion
#pragma region L=4000
	case 4000:
		fh = 1.865;
		break;
#pragma endregion
#pragma region L=4100
	case 4100:
		fh = 1.880;
		break;
#pragma endregion
#pragma region L=4200
	case 4200:
		fh = 1.895;
		break;
#pragma endregion
#pragma region L=4300
	case 4300:
		fh = 1.905;
		break;
#pragma endregion
#pragma region L=4400
	case 4400:
		fh = 1.920;
		break;
#pragma endregion
#pragma region L=4500
	case 4500:
		fh = 1.935;
		break;
#pragma endregion
#pragma region L=4600
	case 4600:
		fh = 1.945;
		break;
#pragma endregion
#pragma region L=4700
	case 4700:
		fh = 1.960;
		break;
#pragma endregion
#pragma region L=4800
	case 4800:
		fh = 1.970;
		break;
#pragma endregion
#pragma region L=4900
	case 4900:
		fh = 1.985;
		break;
#pragma endregion
#pragma region L=5000
	case 5000:
		fh = 2.00;
		break;
#pragma endregion
#pragma region L=5200
	case 5200:
		fh = 2.02;
		break;
#pragma endregion
#pragma region L=5400
	case 5400:
		fh = 2.04;
		break;
#pragma endregion
#pragma region L=5600
	case 5600:
		fh = 2.06;
		break;
#pragma endregion
#pragma region L=5800
	case 5800:
		fh = 2.09;
		break;
#pragma endregion
#pragma region L=6000
	case 6000:
		fh = 2.11;
		break;
#pragma endregion
#pragma region L=6200
	case 6200:
		fh = 2.13;
		break;
#pragma endregion
#pragma region L=6400
	case 6400:
		fh = 2.15;
		break;
#pragma endregion
#pragma region L=6600
	case 6600:
		fh = 2.17;
		break;
#pragma endregion
#pragma region L=6800
	case 6800:
		fh = 2.19;
		break;
#pragma endregion
#pragma region L=7000
	case 7000:
		fh = 2.21;
		break;
#pragma endregion
#pragma region L=7200
	case 7200:
		fh = 2.23;
		break;
#pragma endregion
#pragma region L=7400
	case 7400:
		fh = 2.24;
		break;
#pragma endregion
#pragma region L=7600
	case 7600:
		fh = 2.26;
		break;
#pragma endregion
#pragma region L=7800
	case 7800:
		fh = 2.28;
		break;
#pragma endregion
#pragma region L=8000
	case 8000:
		fh = 2.30;
		break;
#pragma endregion
#pragma region L=8200
	case 8200:
		fh = 2.31;
		break;
#pragma endregion
#pragma region L=8400
	case 8400:
		fh = 2.33;
		break;
#pragma endregion
#pragma region L=8600
	case 8600:
		fh = 2.35;
		break;
#pragma endregion
#pragma region L=8800
	case 8800:
		fh = 2.36;
		break;
#pragma endregion
#pragma region L=9000
	case 9000:
		fh = 2.38;
		break;
#pragma endregion
#pragma region L=9200
	case 9200:
		fh = 2.40;
		break;
#pragma endregion
#pragma region L=9400
	case 9400:
		fh = 2.41;
		break;
#pragma endregion
#pragma region L=9600
	case 9600:
		fh = 2.43;
		break;
#pragma endregion
#pragma region L=9800
	case 9800:
		fh = 2.44;
		break;
#pragma endregion
#pragma region L=10000
	case 10000:
		fh = 2.46;
		break;
#pragma endregion
#pragma region L=10500
	case 10500:
		fh = 2.49;
		break;
#pragma endregion
#pragma region L=11000
	case 11000:
		fh = 2.53;
		break;
#pragma endregion
#pragma region L=11500
	case 11500:
		fh = 2.56;
		break;
#pragma endregion
#pragma region L=12000
	case 12000:
		fh = 2.59;
		break;
#pragma endregion
#pragma region L=12500
	case 12500:
		fh = 2.63;
		break;
#pragma endregion
#pragma region L=13000
	case 13000:
		fh = 2.66;
		break;
#pragma endregion
#pragma region L=13500
	case 13500:
		fh = 2.69;
		break;
#pragma endregion
#pragma region L=14000
	case 14000:
		fh = 2.72;
		break;
#pragma endregion
#pragma region L=14500
	case 14500:
		fh = 2.75;
		break;
#pragma endregion
#pragma region L=15000
	case 15000:
		fh = 2.77;
		break;
#pragma endregion
#pragma region L=15500
	case 15500:
		fh = 2.80;
		break;
#pragma endregion
#pragma region L=16000
	case 16000:
		fh = 2.83;
		break;
#pragma endregion
#pragma region L=16500
	case 16500:
		fh = 2.85;
		break;
#pragma endregion
#pragma region L=17000
	case 17000:
		fh = 2.88;
		break;
#pragma endregion
#pragma region L=17500
	case 17500:
		fh = 2.91;
		break;
#pragma endregion
#pragma region L=18000
	case 18000:
		fh = 2.93;
		break;
#pragma endregion
#pragma region L=18500
	case 18500:
		fh = 2.95;
		break;
#pragma endregion
#pragma region L=19000
	case 19000:
		fh = 2.98;
		break;
#pragma endregion
#pragma region L=19500
	case 19500:
		fh = 3.00;
		break;
#pragma endregion
#pragma region L=20000
	case 20000:
		fh = 3.02;
		break;
#pragma endregion
#pragma region L=21000
	case 21000:
		fh = 3.07;
		break;
#pragma endregion
#pragma region L=22000
	case 22000:
		fh = 3.11;
		break;
#pragma endregion
#pragma region L=23000
	case 23000:
		fh = 3.15;
		break;
#pragma endregion
#pragma region L=24000
	case 24000:
		fh = 3.19;
		break;
#pragma endregion
#pragma region L=25000
	case 25000:
		fh = 3.23;
		break;
#pragma endregion
#pragma region L=26000
	case 26000:
		fh = 3.27;
		break;
#pragma endregion
#pragma region L=27000
	case 27000:
		fh = 3.31;
		break;
#pragma endregion
#pragma region L=28000
	case 28000:
		fh = 3.35;
		break;
#pragma endregion
#pragma region L=29000
	case 29000:
		fh = 3.38;
		break;
#pragma endregion
#pragma region L=30000
	case 30000:
		fh = 3.42;
		break;
#pragma endregion
#pragma region L=31000
	case 31000:
		fh = 3.45;
		break;
#pragma endregion
#pragma region L=32000
	case 32000:
		fh = 3.48;
		break;
#pragma endregion
#pragma region L=33000
	case 33000:
		fh = 3.51;
		break;
#pragma endregion
#pragma region L=34000
	case 34000:
		fh = 3.55;
		break;
#pragma endregion
#pragma region L=35000
	case 35000:
		fh = 3.58;
		break;
#pragma endregion
#pragma region L=36000
	case 36000:
		fh = 3.61;
		break;
#pragma endregion
#pragma region L=37000
	case 37000:
		fh = 3.64;
		break;
#pragma endregion
#pragma region L=38000
	case 38000:
		fh = 3.67;
		break;
#pragma endregion
#pragma region L=39000
	case 39000:
		fh = 3.70;
		break;
#pragma endregion
#pragma region L=40000
	case 40000:
		fh = 3.72;
		break;
#pragma endregion
#pragma region L=41000
	case 41000:
		fh = 3.75;
		break;
#pragma endregion
#pragma region L=42000
	case 42000:
		fh = 3.78;
		break;
#pragma endregion
#pragma region L=43000
	case 43000:
		fh = 3.80;
		break;
#pragma endregion
#pragma region L=44000
	case 44000:
		fh = 3.83;
		break;
#pragma endregion
#pragma region L=45000
	case 45000:
		fh = 3.86;
		break;
#pragma endregion
#pragma region L=46000
	case 46000:
		fh = 3.88;
		break;
#pragma endregion
#pragma region L=47000
	case 47000:
		fh = 3.91;
		break;
#pragma endregion
#pragma region L=48000
	case 48000:
		fh = 3.93;
		break;
#pragma endregion
#pragma region L=49000
	case 49000:
		fh = 3.96;
		break;
#pragma endregion
#pragma region L=50000
	case 50000:
		fh = 3.98;
		break;
#pragma endregion
#pragma region L=55000
	case 55000:
		fh = 4.10;
		break;
#pragma endregion
#pragma region L=60000
	case 60000:
		fh = 4.20;
		break;
#pragma endregion
#pragma region L=65000
	case 65000:
		fh = 4.30;
		break;
#pragma endregion
#pragma region L=70000
	case 70000:
		fh = 4.40;
		break;
#pragma endregion
#pragma region L=75000
	case 75000:
		fh = 4.50;
		break;
#pragma endregion
#pragma region L=80000
	case 80000:
		fh = 4.58;
		break;
#pragma endregion
#pragma region L=85000
	case 85000:
		fh = 4.68;
		break;
#pragma endregion
#pragma region L=90000
	case 90000:
		fh = 4.75;
		break;
#pragma endregion
#pragma region L=100000
	case 100000:
		fh = 4.90;
		break;
#pragma endregion
	default:
		fh = -1;
		
		break;
	}
#pragma endregion
	return fh;
}

double CBearingTAN::fnSet(double n2)
{
#pragma region fn
	int n1= round(n2); int n{ 0 };
	if (n1 >= 10 && n1 <= 50) n = n1;
	if (n1 > 50 && n1 <= 100) n = (n1 / 2) * 2;
	if (n1 > 100 && n1 <= 200) n = (n1 / 5) * 5;
	if (n1 > 200 && n1 <= 500) n = (n1 / 10) * 10;
	if (n1 > 500 && n1 <= 1000) n = (n1 / 20) * 20;
	if (n1 > 1000 && n1 <= 2000) n = (n1 / 50) * 50;
	if (n1 > 2000 && n1 <= 5000) n = (n1 / 100) * 100;
	if (n1 > 5000 && n1 <= 10000) n = (n1 / 200) * 200;
	if (n1 > 10000 && n1 <= 20000) n = (n1 / 500) * 500;
	if (n1 > 20000 && n1 <= 30000) n = (n1 / 1000) * 1000;

	switch (n)  //查表求fn值
	{
#pragma region n=10
	case 10:
		fn = 1.435;
		break;
#pragma endregion
#pragma region n=11
	case 11:
		fn = 1.395;
		break;
#pragma endregion
#pragma region n=12
	case 12:
		fn = 1.359;
		break;
#pragma endregion
#pragma region n=13
	case 13:
		fn = 1.326;
		break;
#pragma endregion
#pragma region n=14
	case 14:
		fn = 1.297;
		break;
#pragma endregion
#pragma region n=15
	case 15:
		fn = 1.271;
		break;
#pragma endregion
#pragma region n=16
	case 16:
		fn = 1.246;
		break;
#pragma endregion
#pragma region n=17
	case 17:
		fn = 1.224;
		break;
#pragma endregion
#pragma region n=18
	case 18:
		fn = 1.203;
		break;
#pragma endregion
#pragma region n=19
	case 19:
		fn = 1.184;
		break;
#pragma endregion
#pragma region n=20
	case 20:
		fn = 1.166;
		break;
#pragma endregion
#pragma region n=21
	case 21:
		fn = 1.149;
		break;
#pragma endregion
#pragma region n=22
	case 22:
		fn = 1.133;
		break;
#pragma endregion
#pragma region n=23
	case 23:
		fn = 1.118;
		break;
#pragma endregion
#pragma region n=24
	case 24:
		fn = 1.104;
		break;
#pragma endregion
#pragma region n=25
	case 25:
		fn = 1.090;
		break;
#pragma endregion
#pragma region n=26
	case 26:
		fn = 1.077;
		break;
#pragma endregion
#pragma region n=27
	case 27:
		fn = 1.065;
		break;
#pragma endregion
#pragma region n=28
	case 28:
		fn = 1.054;
		break;
#pragma endregion
#pragma region n=29
	case 29:
		fn = 1.043;
		break;
#pragma endregion
#pragma region n=30
	case 30:
		fn = 1.032;
		break;
#pragma endregion
#pragma region n=31
	case 31:
		fn = 1.022;
		break;
#pragma endregion
#pragma region n=32
	case 32:
		fn = 1.012;
		break;
#pragma endregion
#pragma region n=33
	case 33:
		fn = 1.003;
		break;
#pragma endregion
#pragma region n=34
	case 34:
		fn = 0.994;
		break;
#pragma endregion
#pragma region n=35
	case 35:
		fn = 0.985;
		break;
#pragma endregion
#pragma region n=36
	case 36:
		fn = 0.977;
		break;
#pragma endregion
#pragma region n=37
	case 37:
		fn = 0.969;
		break;
#pragma endregion
#pragma region n=38
	case 38:
		fn = 0.961;
		break;
#pragma endregion
#pragma region n=39
	case 39:
		fn = 0.954;
		break;
#pragma endregion
#pragma region n=40
	case 40:
		fn = 0.947;
		break;
#pragma endregion
#pragma region n=41
	case 41:
		fn = 0.940;
		break;
#pragma endregion
#pragma region n=42
	case 42:
		fn = 0.933;
		break;
#pragma endregion
#pragma region n=43
	case 43:
		fn = 0.927;
		break;
#pragma endregion
#pragma region n=44
	case 44:
		fn = 0.920;
		break;
#pragma endregion
#pragma region n=45
	case 45:
		fn = 0.914;
		break;
#pragma endregion
#pragma region n=46
	case 46:
		fn = 0.908;
		break;
#pragma endregion
#pragma region n=47
	case 47:
		fn = 0.902;
		break;
#pragma endregion
#pragma region n=48
	case 48:
		fn = 0.896;
		break;
#pragma endregion
#pragma region n=49
	case 49:
		fn = 0.891;
		break;
#pragma endregion
#pragma region n=50
	case 50:
		fn = 0.885;
		break;
#pragma endregion
#pragma region n=52
	case 52:
		fn = 0.875;
		break;
#pragma endregion
#pragma region n=54
	case 54:
		fn = 0.865;
		break;
#pragma endregion
#pragma region n=56
	case 56:
		fn = 0.856;
		break;
#pragma endregion
#pragma region n=58
	case 58:
		fn = 0.847;
		break;
#pragma endregion
#pragma region n=60
	case 60:
		fn = 0.838;
		break;
#pragma endregion
#pragma region n=62
	case 62:
		fn = 0.830;
		break;
#pragma endregion
#pragma region n=64
	case 64:
		fn = 0.822;
		break;
#pragma endregion
#pragma region n=66
	case 66:
		fn = 0.815;
		break;
#pragma endregion
#pragma region n=68
	case 68:
		fn = 0.807;
		break;
#pragma endregion
#pragma region n=70
	case 70:
		fn = 0.800;
		break;
#pragma endregion
#pragma region n=72
	case 72:
		fn = 0.794;
		break;
#pragma endregion
#pragma region n=74
	case 74:
		fn = 0.787;
		break;
#pragma endregion
#pragma region n=76
	case 76:
		fn = 0.781;
		break;
#pragma endregion
#pragma region n=78
	case 78:
		fn = 0.775;
		break;
#pragma endregion
#pragma region n=80
	case 80:
		fn = 0.769;
		break;
#pragma endregion
#pragma region n=82
	case 82:
		fn = 0.763;
		break;
#pragma endregion
#pragma region n=84
	case 84:
		fn = 0.758;
		break;
#pragma endregion
#pragma region n=86
	case 86:
		fn = 0.753;
		break;
#pragma endregion
#pragma region n=88
	case 88:
		fn = 0.747;
		break;
#pragma endregion
#pragma region n=90
	case 90:
		fn = 0.742;
		break;
#pragma endregion
#pragma region n=92
	case 92:
		fn = 0.737;
		break;
#pragma endregion
#pragma region n=94
	case 94:
		fn = 0.733;
		break;
#pragma endregion
#pragma region n=96
	case 96:
		fn = 0.728;
		break;
#pragma endregion
#pragma region n=98
	case 98:
		fn = 0.724;
		break;
#pragma endregion
#pragma region n=100
	case 100:
		fn = 0.719;
		break;
#pragma endregion
#pragma region n=105
	case 105:
		fn = 0.709;
		break;
#pragma endregion
#pragma region n=110
	case 110:
		fn = 0.699;
		break;
#pragma endregion
#pragma region n=115
	case 115:
		fn = 0.690;
		break;
#pragma endregion
#pragma region n=120
	case 120:
		fn = 0.681;
		break;
#pragma endregion
#pragma region n=125
	case 125:
		fn = 0.673;
		break;
#pragma endregion
#pragma region n=130
	case 130:
		fn = 0.665;
		break;
#pragma endregion
#pragma region n=135
	case 135:
		fn = 0.657;
		break;
#pragma endregion
#pragma region n=140
	case 140:
		fn = 0.650;
		break;
#pragma endregion
#pragma region n=145
	case 145:
		fn = 0.643;
		break;
#pragma endregion
#pragma region n=150
	case 150:
		fn = 0.637;
		break;
#pragma endregion
#pragma region n=155
	case 155:
		fn = 0.631;
		break;
#pragma endregion
#pragma region n=160
	case 160:
		fn = 0.625;
		break;
#pragma endregion
#pragma region n=165
	case 165:
		fn = 0.619;
		break;
#pragma endregion
#pragma region n=170
	case 170:
		fn = 0.613;
		break;
#pragma endregion
#pragma region n=175
	case 175:
		fn = 0.608;
		break;
#pragma endregion
#pragma region n=180
	case 180:
		fn = 0.603;
		break;
#pragma endregion
#pragma region n=185
	case 185:
		fn = 0.598;
		break;
#pragma endregion
#pragma region n=190
	case 190:
		fn = 0.593;
		break;
#pragma endregion
#pragma region n=195
	case 195:
		fn = 0.589;
		break;
#pragma endregion
#pragma region n=200
	case 200:
		fn = 0.584;
		break;
#pragma endregion
#pragma region n=210
	case 210:
		fn = 0.576;
		break;
#pragma endregion
#pragma region n=220
	case 220:
		fn = 0.568;
		break;
#pragma endregion
#pragma region n=230
	case 230:
		fn = 0.560;
		break;
#pragma endregion
#pragma region n=240
	case 240:
		fn = 0.553;
		break;
#pragma endregion
#pragma region n=250
	case 250:
		fn = 0.546;
		break;
#pragma endregion
#pragma region n=260
	case 260:
		fn = 0.540;
		break;
#pragma endregion
#pragma region n=270
	case 270:
		fn = 0.534;
		break;
#pragma endregion
#pragma region n=280
	case 280:
		fn = 0.528;
		break;
#pragma endregion
#pragma region n=290
	case 290:
		fn = 0.523;
		break;
#pragma endregion
#pragma region n=300
	case 300:
		fn = 0.517;
		break;
#pragma endregion
#pragma region n=310
	case 310:
		fn = 0.512;
		break;
#pragma endregion
#pragma region n=320
	case 320:
		fn = 0.507;
		break;
#pragma endregion
#pragma region n=330
	case 330:
		fn = 0.503;
		break;
#pragma endregion
#pragma region n=340
	case 340:
		fn = 0.498;
		break;
#pragma endregion
#pragma region n=350
	case 350:
		fn = 0.494;
		break;
#pragma endregion
#pragma region n=360
	case 360:
		fn = 0.490;
		break;
#pragma endregion
#pragma region n=370
	case 370:
		fn = 0.486;
		break;
#pragma endregion
#pragma region n=380
	case 380:
		fn = 0.482;
		break;
#pragma endregion
#pragma region n=390
	case 390:
		fn = 0.478;
		break;
#pragma endregion
#pragma region n=400
	case 400:
		fn = 0.475;
		break;
#pragma endregion
#pragma region n=410
	case 410:
		fn = 0.471;
		break;
#pragma endregion
#pragma region n=420
	case 420:
		fn = 0.467;
		break;
#pragma endregion
#pragma region n=430
	case 430:
		fn = 0.464;
		break;
#pragma endregion
#pragma region n=440
	case 440:
		fn = 0.461;
		break;
#pragma endregion
#pragma region n=450
	case 450:
		fn = 0.458;
		break;
#pragma endregion
#pragma region n=460
	case 460:
		fn = 0.455;
		break;
#pragma endregion
#pragma region n=470
	case 470:
		fn = 0.452;
		break;
#pragma endregion
#pragma region n=480
	case 480:
		fn = 0.449;
		break;
#pragma endregion
#pragma region n=490
	case 490:
		fn = 0.447;
		break;
#pragma endregion
#pragma region n=500
	case 500:
		fn = 0.444;
		break;
#pragma endregion
#pragma region n=520
	case 520:
		fn = 0.439;
		break;
#pragma endregion
#pragma region n=540
	case 540:
		fn = 0.434;
		break;
#pragma endregion
#pragma region n=560
	case 560:
		fn = 0.429;
		break;
#pragma endregion
#pragma region n=580
	case 580:
		fn = 0.424;
		break;
#pragma endregion
#pragma region n=600
	case 600:
		fn = 0.420;
		break;
#pragma endregion
#pragma region n=620
	case 620:
		fn = 0.416;
		break;
#pragma endregion
#pragma region n=640
	case 640:
		fn = 0.412;
		break;
#pragma endregion
#pragma region n=660
	case 660:
		fn = 0.408;
		break;
#pragma endregion
#pragma region n=680
	case 680:
		fn = 0.405;
		break;
#pragma endregion
#pragma region n=700
	case 700:
		fn = 0.401;
		break;
#pragma endregion
#pragma region n=720
	case 720:
		fn = 0.398;
		break;
#pragma endregion
#pragma region n=740
	case 740:
		fn = 0.395;
		break;
#pragma endregion
#pragma region n=760
	case 760:
		fn = 0.391;
		break;
#pragma endregion
#pragma region n=780
	case 780:
		fn = 0.388;
		break;
#pragma endregion
#pragma region n=800
	case 800:
		fn = 0.385;
		break;
#pragma endregion
#pragma region n=820
	case 820:
		fn = 0.383;
		break;
#pragma endregion
#pragma region n=840
	case 840:
		fn = 0.380;
		break;
#pragma endregion
#pragma region n=860
	case 860:
		fn = 0.377;
		break;
#pragma endregion
#pragma region n=880
	case 880:
		fn = 0.375;
		break;
#pragma endregion
#pragma region n=900
	case 900:
		fn = 0.372;
		break;
#pragma endregion
#pragma region n=920
	case 920:
		fn = 0.370;
		break;
#pragma endregion
#pragma region n=940
	case 940:
		fn = 0.367;
		break;
#pragma endregion
#pragma region n=960
	case 960:
		fn = 0.366;
		break;
#pragma endregion
#pragma region n=980
	case 980:
		fn = 0.363;
		break;
#pragma endregion
#pragma region n=1000
	case 1000:
		fn = 0.360;
		break;
#pragma endregion
#pragma region n=1050
	case 1050:
		fn = 0.355;
		break;
#pragma endregion
#pragma region n=1100
	case 1100:
		fn = 0.350;
		break;
#pragma endregion
#pragma region n=1150
	case 1150:
		fn = 0.346;
		break;
#pragma endregion
#pragma region n=1200
	case 1200:
		fn = 0.341;
		break;
#pragma endregion
#pragma region n=1250
	case 1250:
		fn = 0.337;
		break;
#pragma endregion
#pragma region n=1300
	case 1300:
		fn = 0.333;
		break;
#pragma endregion
#pragma region n=1350
	case 1350:
		fn = 0.329;
		break;
#pragma endregion
#pragma region n=1400
	case 1400:
		fn = 0.326;
		break;
#pragma endregion
#pragma region n=1450
	case 1450:
		fn = 0.322;
		break;
#pragma endregion
#pragma region n=1500
	case 1500:
		fn = 0.319;
		break;
#pragma endregion
#pragma region n=1550
	case 1550:
		fn = 0.316;
		break;
#pragma endregion
#pragma region n=1600
	case 1600:
		fn = 0.313;
		break;
#pragma endregion
#pragma region n=1650
	case 1650:
		fn = 0.310;
		break;
#pragma endregion
#pragma region n=1700
	case 1700:
		fn = 0.307;
		break;
#pragma endregion
#pragma region n=1750
	case 1750:
		fn = 0.305;
		break;
#pragma endregion
#pragma region n=1800
	case 1800:
		fn = 0.302;
		break;
#pragma endregion
#pragma region n=1850
	case 1850:
		fn = 0.300;
		break;
#pragma endregion
#pragma region n=1900
	case 1900:
		fn = 0.297;
		break;
#pragma endregion
#pragma region n=1950
	case 1950:
		fn = 0.295;
		break;
#pragma endregion
#pragma region n=2000
	case 2000:
		fn = 0.293;
		break;
#pragma endregion
#pragma region n=2100
	case 2100:
		fn = 0.289;
		break;
#pragma endregion
#pragma region n=2200
	case 2200:
		fn = 0.285;
		break;
#pragma endregion
#pragma region n=2300
	case 2300:
		fn = 0.281;
		break;
#pragma endregion
#pragma region n=2400
	case 2400:
		fn = 0.277;
		break;
#pragma endregion
#pragma region n=2500
	case 2500:
		fn = 0.274;
		break;
#pragma endregion
#pragma region n=2600
	case 2600:
		fn = 0.271;
		break;
#pragma endregion
#pragma region n=2700
	case 2700:
		fn = 0.268;
		break;
#pragma endregion
#pragma region n=2800
	case 2800:
		fn = 0.265;
		break;
#pragma endregion
#pragma region n=2900
	case 2900:
		fn = 0.262;
		break;
#pragma endregion
#pragma region n=3000
	case 3000:
		fn = 0.259;
		break;
#pragma endregion
#pragma region n=3100
	case 3100:
		fn = 0.257;
		break;
#pragma endregion
#pragma region n=3200
	case 3200:
		fn = 0.254;
		break;
#pragma endregion
#pragma region n=3300
	case 3300:
		fn = 0.252;
		break;
#pragma endregion
#pragma region n=3400
	case 3400:
		fn = 0.250;
		break;
#pragma endregion
#pragma region n=3500
	case 3500:
		fn = 0.248;
		break;
#pragma endregion
#pragma region n=3600
	case 3600:
		fn = 0.246;
		break;
#pragma endregion
#pragma region n=3700
	case 3700:
		fn = 0.243;
		break;
#pragma endregion
#pragma region n=3800
	case 3800:
		fn = 0.242;
		break;
#pragma endregion
#pragma region n=3900
	case 3900:
		fn = 0.240;
		break;
#pragma endregion
#pragma region n=4000
	case 4000:
		fn = 0.238;
		break;
#pragma endregion
#pragma region n=4100
	case 4100:
		fn = 0.236;
		break;
#pragma endregion
#pragma region n=4200
	case 4200:
		fn = 0.234;
		break;
#pragma endregion
#pragma region n=4300
	case 4300:
		fn = 0.233;
		break;
#pragma endregion
#pragma region n=4400
	case 4400:
		fn = 0.231;
		break;
#pragma endregion
#pragma region n=4500
	case 4500:
		fn = 0.230;
		break;
#pragma endregion
#pragma region n=4600
	case 4600:
		fn = 0.228;
		break;
#pragma endregion
#pragma region n=4700
	case 4700:
		fn = 0.227;
		break;
#pragma endregion
#pragma region n=4800
	case 4800:
		fn = 0.225;
		break;
#pragma endregion
#pragma region n=4900
	case 4900:
		fn = 0.224;
		break;
#pragma endregion
#pragma region n=5000
	case 5000:
		fn = 0.222;
		break;
#pragma endregion
#pragma region n=5200
	case 5200:
		fn = 0.220;
		break;
#pragma endregion
#pragma region n=5400
	case 5400:
		fn = 0.217;
		break;
#pragma endregion
#pragma region n=5600
	case 5600:
		fn = 0.215;
		break;
#pragma endregion
#pragma region n=5800
	case 5800:
		fn = 0.213;
		break;
#pragma endregion
#pragma region n=6000
	case 6000:
		fn = 0.211;
		break;
#pragma endregion
#pragma region n=6200
	case 6200:
		fn = 0.209;
		break;
#pragma endregion
#pragma region n=6400
	case 6400:
		fn = 0.207;
		break;
#pragma endregion
#pragma region n=6600
	case 6600:
		fn = 0.205;
		break;
#pragma endregion
#pragma region n=6800
	case 6800:
		fn = 0.203;
		break;
#pragma endregion
#pragma region n=7000
	case 7000:
		fn = 0.201;
		break;
#pragma endregion
#pragma region n=7200
	case 7200:
		fn = 0.199;
		break;
#pragma endregion
#pragma region n=7400
	case 7400:
		fn = 0.198;
		break;
#pragma endregion
#pragma region n=7600
	case 7600:
		fn = 0.196;
		break;
#pragma endregion
#pragma region n=7800
	case 7800:
		fn = 0.195;
		break;
#pragma endregion
#pragma region n=8000
	case 8000:
		fn = 0.193;
		break;
#pragma endregion
#pragma region n=8200
	case 8200:
		fn = 0.192;
		break;
#pragma endregion
#pragma region n=8400
	case 8400:
		fn = 0.190;
		break;
#pragma endregion
#pragma region n=8600
	case 8600:
		fn = 0.189;
		break;
#pragma endregion
#pragma region n=8800
	case 8800:
		fn = 0.188;
		break;
#pragma endregion
#pragma region n=9000
	case 9000:
		fn = 0.187;
		break;
#pragma endregion
#pragma region n=9200
	case 9200:
		fn = 0.185;
		break;
#pragma endregion
#pragma region n=9400
	case 9400:
		fn = 0.184;
		break;
#pragma endregion
#pragma region n=9600
	case 9600:
		fn = 0.183;
		break;
#pragma endregion
#pragma region n=9800
	case 9800:
		fn = 0.182;
		break;
#pragma endregion
#pragma region n=10000
	case 10000:
		fn = 0.181;
		break;
#pragma endregion
#pragma region n=10500
	case 10500:
		fn = 0.178;
		break;
#pragma endregion
#pragma region n=11000
	case 11000:
		fn = 0.176;
		break;
#pragma endregion
#pragma region n=11500
	case 11500:
		fn = 0.173;
		break;
#pragma endregion
#pragma region n=12000
	case 12000:
		fn = 0.171;
		break;
#pragma endregion
#pragma region n=12500
	case 12500:
		fn = 0.169;
		break;
#pragma endregion
#pragma region n=13000
	case 13000:
		fn = 0.167;
		break;
#pragma endregion
#pragma region n=13500
	case 13500:
		fn = 0.165;
		break;
#pragma endregion
#pragma region n=14000
	case 14000:
		fn = 0.163;
		break;
#pragma endregion
#pragma region n=14500
	case 14500:
		fn = 0.162;
		break;
#pragma endregion
#pragma region n=15000
	case 15000:
		fn = 0.160;
		break;
#pragma endregion
#pragma region n=15500
	case 15500:
		fn = 0.158;
		break;
#pragma endregion
#pragma region n=16000
	case 16000:
		fn = 0.157;
		break;
#pragma endregion
#pragma region n=16500
	case 16500:
		fn = 0.155;
		break;
#pragma endregion
#pragma region n=17000
	case 17000:
		fn = 0.154;
		break;
#pragma endregion
#pragma region n=17500
	case 17500:
		fn = 0.153;
		break;
#pragma endregion
#pragma region n=18000
	case 18000:
		fn = 0.151;
		break;
#pragma endregion
#pragma region n=18500
	case 18500:
		fn = 0.150;
		break;
#pragma endregion
#pragma region n=19000
	case 19000:
		fn = 0.149;
		break;
#pragma endregion
#pragma region n=19500
	case 19500:
		fn = 0.148;
		break;
#pragma endregion
#pragma region n=20000
	case 20000:
		fn = 0.147;
		break;
#pragma endregion
#pragma region n=21000
	case 21000:
		fn = 0.146;
		break;
#pragma endregion
#pragma region n=22000
	case 22000:
		fn = 0.143;
		break;
#pragma endregion
#pragma region n=23000
	case 23000:
		fn = 0.141;
		break;
#pragma endregion
#pragma region n=24000
	case 24000:
		fn = 0.139;
		break;
#pragma endregion
#pragma region n=25000
	case 25000:
		fn = 0.137;
		break;
#pragma endregion
#pragma region n=26000
	case 26000:
		fn = 0.136;
		break;
#pragma endregion
#pragma region n=27000
	case 27000:
		fn = 0.134;
		break;
#pragma endregion
#pragma region n=28000
	case 28000:
		fn = 0.133;
		break;
#pragma endregion
#pragma region n=29000
	case 29000:
		fn = 0.131;
		break;
#pragma endregion
#pragma region n=30000
	case 30000:
		fn = 0.130;
		break;
#pragma endregion
	default:
		fn = -1;
		
		break;
	}
#pragma endregion
	return fn;
}

void CBearingTAN::Para_switch(int InInnerDiameter,int InOuterDiameter)
{
	switch (InInnerDiameter)
	{
#pragma region  d=15
	case 15:
		if (InOuterDiameter == 42)
		{
			Cr = 23.8;
			Cor = 21.5;
			e = 0.29;
			Y = 2.1;
			Yo = 1.2;
			stype = 30302;
		}
		else
		{
			stype = -1;
			
		}
		break;
#pragma endregion
#pragma region  d=17
	case 17:
		switch (InOuterDiameter)
		{
#pragma region  D=40
		case 40:
			Cr = 21.8;
			Cor = 21.8;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 30203;
			break;
#pragma endregion
#pragma region  D=47
		case 47:
			Cr = 36.8;
			Cor = 36.2;
			e = 0.29;
			Y = 2.1;
			Yo = 1.2;
			stype = 32303;
			break;
#pragma endregion

		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=20
	case 20:
		switch (InOuterDiameter)
		{
#pragma region  D=37
		case 37:
			Cr = 13.8;
			Cor = 17.5;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 32904;
			break;
#pragma endregion
#pragma region  D=42
		case 42:
			Cr = 26.2;
			Cor = 28.2;
			e = 0.37;
			Y = 1.6;
			Yo = 0.9;
			stype = 32004;
			break;
#pragma endregion
#pragma region  D=47
		case 47:
			Cr = 29.5;
			Cor = 30.5;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 30204;
			break;
#pragma endregion
#pragma region  D=52
		case 52:
			Cr = 44.8;
			Cor = 46.2;
			e = 0.3;
			Y = 2;
			Yo = 1.1;
			stype = 32304;
			break;
#pragma endregion
		default:
			stype = -1;
			break;
		}
		break;
#pragma endregion
#pragma region  d=22
	case 22:
		switch (InOuterDiameter)
		{
#pragma region  D=40
		case 40:
			Cr = 15.8;
			Cor = 20.0;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 329122;
			break;
#pragma endregion
#pragma region  D=44
		case 44:
			Cr = 27.2;
			Cor = 30.2;
			e = 0.40;
			Y = 1.5;
			Yo = 0.8;
			stype = 320122;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=25
	case 25:
		switch (InOuterDiameter)
		{
#pragma region  D=42
		case 42:
			Cr = 16.8;
			Cor = 21.0;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 32905;
			break;
#pragma endregion
#pragma region  D=47
		case 47:
			Cr = 34.0;
			Cor = 42.5;
			e = 0.29;
			Y = 2.1;
			Yo = 1.1;
			stype = 33005;
			break;
#pragma endregion
#pragma region  D=52
		case 52:
			Cr = 49.2;
			Cor = 55.8;
			e = 0.35;
			Y = 1.7;
			Yo = 0.9;
			stype = 33205;
			break;
#pragma endregion
#pragma region  D=62
		case 62:
			Cr = 64.5;
			Cor = 68.8;
			e = 0.3;
			Y = 2;
			Yo = 1.1;
			stype = 32305;
			break;
#pragma endregion

		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=28
	case 28:
		switch (InOuterDiameter)
		{
#pragma region  D=45
		case 45:
			Cr = 17.5;
			Cor = 22.8;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 329128;
			break;
#pragma endregion
#pragma region  D=52
		case 52:
			Cr = 33.0;
			Cor = 40.5;
			e = 0.43;
			Y = 1.4;
			Yo = 0.8;
			stype = 320128;
			break;
#pragma endregion
#pragma region  D=58
		case 58:
			Cr = 60.8;
			Cor = 68.2;
			e = 0.34;
			Y = 1.8;
			Yo = 1.0;
			stype = 332128;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=30
	case 30:
		switch (InOuterDiameter)
		{
#pragma region  D=47
		case 47:
			Cr = 17.8;
			Cor = 23.2;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 32906;
			break;
#pragma endregion
#pragma region  D=55
		case 55:
			Cr = 45.8;
			Cor = 58.8;
			e = 0.29;
			Y = 2.1;
			Yo = 1.1;
			stype = 33006;
			break;
#pragma endregion
#pragma region  D=62
		case 62:
			Cr = 66.8;
			Cor = 75.5;
			e = 0.34;
			Y = 1.8;
			Yo = 1;
			stype = 33206;
			break;
#pragma endregion
#pragma region  D=72
		case 72:
			Cr = 85.5;
			Cor = 96.5;
			e = 0.31;
			Y = 1.9;
			Yo = 1.1;
			stype = 32306;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=32
	case 32:
		switch (InOuterDiameter)
		{
#pragma region  D=52
		case 52:
			Cr = 25.0;
			Cor = 32.5;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 329132;
			break;
#pragma endregion
#pragma region  D=58
		case 58:
			Cr = 38.2;
			Cor = 49.2;
			e = 0.45;
			Y = 1.3;
			Yo = 0.7;
			stype = 320132;
			break;
#pragma endregion
#pragma region  D=65
		case 65:
			Cr = 72.0;
			Cor = 82.2;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 332132;
			break;
#pragma endregion

		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=35
	case 35:
		switch (InOuterDiameter)
		{
#pragma region  D=55
		case 55:
			Cr = 27.0;
			Cor = 34.8;
			e = 0.29;
			Y = 2.1;
			Yo = 1.1;
			stype = 32907;
			break;
#pragma endregion
#pragma region  D=62
		case 62:
			Cr = 49.0;
			Cor = 63.2;
			e = 0.31;
			Y = 2;
			Yo = 1.1;
			stype = 33007;
			break;
#pragma endregion
#pragma region  D=72
		case 72:
			Cr = 86.5;
			Cor = 102;
			e = 0.35;
			Y = 1.7;
			Yo = 0.9;
			stype = 33207;
			break;
#pragma endregion
#pragma region  D=80
		case 80:
			Cr = 105;
			Cor = 118;
			e = 0.31;
			Y = 1.9;
			Yo = 1.1;
			stype = 32307;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=40
	case 40:
		switch (InOuterDiameter)
		{
#pragma region  D=62
		case 62:
			Cr = 33.0;
			Cor = 46.0;
			e = 0.29;
			Y = 2.1;
			Yo = 1.1;
			stype = 32908;
			break;
#pragma endregion
#pragma region  D=68
		case 68:
			Cr = 63.0;
			Cor = 79.5;
			e = 0.28;
			Y = 2.1;
			Yo = 1.2;
			stype = 33008;
			break;
#pragma endregion
#pragma region  D=75
		case 75:
			Cr = 88.8;
			Cor = 110;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 33108;
			break;
#pragma endregion
#pragma region  D=80
		case 80:
			Cr = 110.0;
			Cor = 135;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 33208;
			break;
#pragma endregion
#pragma region  D=90
		case 90:
			Cr = 120;
			Cor = 148;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32308;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=45
	case 45:
		switch (InOuterDiameter)
		{
#pragma region  D=68
		case 68:
			Cr = 33.5;
			Cor = 48.5;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 32909;
			break;
#pragma endregion
#pragma region  D=75
		case 75:
			Cr = 76.0;
			Cor = 100;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 33009;
			break;
#pragma endregion
#pragma region  D=80
		case 80:
			Cr = 91.2;
			Cor = 118;
			e = 0.38;
			Y = 1.6;
			Yo = 1;
			stype = 33109;
			break;
#pragma endregion
#pragma region  D=85
		case 85:
			Cr = 115;
			Cor = 145;
			e = 0.39;
			Y = 1.5;
			Yo = 0.9;
			stype = 33209;
			break;
#pragma endregion
#pragma region  D=100
		case 100:
			Cr = 152;
			Cor = 188;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32309;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=50
	case 50:
		switch (InOuterDiameter)
		{
#pragma region  D=72
		case 72:
			Cr = 38.5;
			Cor = 56.0;
			e = 0.34;
			Y = 1.8;
			Yo = 1;
			stype = 32910;
			break;
#pragma endregion
#pragma region  D=80
		case 80:
			Cr = 80.5;
			Cor = 110;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 33010;
			break;
#pragma endregion
#pragma region  D=85
		case 85:
			Cr = 93.5;
			Cor = 125;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 33110;
			break;
#pragma endregion
#pragma region  D=90
		case 90:
			Cr = 118;
			Cor = 155;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 33210;
			break;
#pragma endregion
#pragma region  D=110
		case 110:
			Cr = 185;
			Cor = 235;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32310;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=55
	case 55:
		switch (InOuterDiameter)
		{
#pragma region  D=80
		case 80:
			Cr = 43.5;
			Cor = 66.8;
			e = 0.31;
			Y = 1.9;
			Yo = 1.1;
			stype = 32911;
			break;
#pragma endregion
#pragma region  D=90
		case 90:
			Cr = 99.2;
			Cor = 145;
			e = 0.31;
			Y = 1.9;
			Yo = 1.1;
			stype = 33011;
			break;
#pragma endregion
#pragma region  D=95
		case 95:
			Cr = 120;
			Cor = 165;
			e = 0.37;
			Y = 1.6;
			Yo = 0.9;
			stype = 33111;
			break;
#pragma endregion
#pragma region  D=100
		case 100:
			Cr = 148;
			Cor = 198;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33211;
			break;
#pragma endregion
#pragma region  D=120
		case 120:
			Cr = 212;
			Cor = 270;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32311;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=60
	case 60:
		switch (InOuterDiameter)
		{
#pragma region  D=85
		case 85:
			Cr = 48.2;
			Cor = 73.0;
			e = 0.33;
			Y = 1.8;
			Yo = 1;
			stype = 32912;
			break;
#pragma endregion
#pragma region  D=95
		case 95:
			Cr = 102;
			Cor = 150;
			e = 0.33;
			Y = 1.8;
			Yo = 1;
			stype = 33012;
			break;
#pragma endregion
#pragma region  D=100
		case 100:
			Cr = 125;
			Cor = 172;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33112;
			break;
#pragma endregion
#pragma region  D=110
		case 110:
			Cr = 172;
			Cor = 230;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33212;
			break;
#pragma endregion
#pragma region  D=130
		case 130:
			Cr = 238;
			Cor = 302;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32312;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=65
	case 65:
		switch (InOuterDiameter)
		{
#pragma region  D=90
		case 90:
			Cr = 47.5;
			Cor = 73.2;
			e = 0.35;
			Y = 1.7;
			Yo = 0.9;
			stype = 32913;
			break;
#pragma endregion
#pragma region  D=100
		case 100:
			Cr = 102;
			Cor = 158;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 33013;
			break;
#pragma endregion
#pragma region  D=110
		case 110:
			Cr = 148;
			Cor = 220;
			e = 0.39;
			Y = 1.6;
			Yo = 0.9;
			stype = 33113;
			break;
#pragma endregion
#pragma region  D=120
		case 120:
			Cr = 212;
			Cor = 282;
			e = 0.39;
			Y = 1.5;
			Yo = 0.9;
			stype = 33213;
			break;
#pragma endregion
#pragma region  D=140
		case 140:
			Cr = 272;
			Cor = 350;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32313;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=70
	case 70:
		switch (InOuterDiameter)
		{
#pragma region  D=100
		case 100:
			Cr = 74.2;
			Cor = 115;
			e = 0.32;
			Y = 1.9;
			Yo = 1;
			stype = 32914;
			break;
#pragma endregion
#pragma region  D=110
		case 110:
			Cr = 142;
			Cor = 220;
			e = 0.28;
			Y = 2;
			Yo = 1;
			stype = 33014;
			break;
#pragma endregion
#pragma region  D=120
		case 120:
			Cr = 180;
			Cor = 268;
			e = 0.39;
			Y = 1.5;
			Yo = 1.2;
			stype = 33114;
			break;
#pragma endregion
#pragma region  D=125
		case 125:
			Cr = 218;
			Cor = 298;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 33214;
			break;
#pragma endregion
#pragma region  D=150
		case 150:
			Cr = 312;
			Cor = 408;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32314;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=75
	case 75:
		switch (InOuterDiameter)
		{
#pragma region  D=105
		case 105:
			Cr = 82.0;
			Cor = 125;
			e = 0.33;
			Y = 1.8;
			Yo = 1;
			stype = 32915;
			break;
#pragma endregion
#pragma region  D=115
		case 115:
			Cr = 138;
			Cor = 220;
			e = 0.3;
			Y = 2;
			Yo = 1;
			stype = 33015;
			break;
#pragma endregion
#pragma region  D=125
		case 125:
			Cr = 182;
			Cor = 280;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33115;
			break;
#pragma endregion
#pragma region  D=130
		case 130:
			Cr = 218;
			Cor = 300;
			e = 0.43;
			Y = 1.4;
			Yo = 0.8;
			stype = 33215;
			break;
#pragma endregion
#pragma region  D=160
		case 160:
			Cr = 365;
			Cor = 482;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32315;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=80
	case 80:
		switch (InOuterDiameter)
		{
#pragma region  D=110
		case 110:
			Cr = 83.0;
			Cor = 128;
			e = 0.35;
			Y = 1.7;
			Yo = 0.9;
			stype = 32916;
			break;
#pragma endregion
#pragma region  D=125
		case 125:
			Cr = 190;
			Cor = 305;
			e = 0.28;
			Y = 2.2;
			Yo = 1.2;
			stype = 33016;
			break;
#pragma endregion
#pragma region  D=130
		case 130:
			Cr = 188;
			Cor = 292;
			e = 0.42;
			Y = 1.4;
			Yo = 0.8;
			stype = 33116;
			break;
#pragma endregion
#pragma region  D=140
		case 140:
			Cr = 258;
			Cor = 362;
			e = 0.43;
			Y = 1.4;
			Yo = 0.8;
			stype = 33216;
			break;
#pragma endregion
#pragma region  D=170
		case 170:
			Cr = 408;
			Cor = 542;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32316;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=85
	case 85:
		switch (InOuterDiameter)
		{
#pragma region  D=120
		case 120:
			Cr = 102;
			Cor = 165;
			e = 0.33;
			Y = 1.8;
			Yo = 1;
			stype = 32917;
			break;
#pragma endregion
#pragma region  D=130
		case 130:
			Cr = 188;
			Cor = 305;
			e = 0.29;
			Y = 2.1;
			Yo = 1.1;
			stype = 33017;
			break;
#pragma endregion
#pragma region  D=140
		case 140:
			Cr = 225;
			Cor = 355;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 33117;
			break;
#pragma endregion
#pragma region  D=150
		case 150:
			Cr = 295;
			Cor = 415;
			e = 0.42;
			Y = 1.4;
			Yo = 0.8;
			stype = 33217;
			break;
#pragma endregion
#pragma region  D=180
		case 180:
			Cr = 442;
			Cor = 592;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32317;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=90
	case 90:
		switch (InOuterDiameter)
		{
#pragma region  D=125
		case 125:
			Cr = 100;
			Cor = 165;
			e = 0.34;
			Y = 1.8;
			Yo = 1;
			stype = 32918;
			break;
#pragma endregion
#pragma region  D=140
		case 140:
			Cr = 242;
			Cor = 388;
			e = 0.27;
			Y = 2.2;
			Yo = 1.2;
			stype = 33018;
			break;
#pragma endregion
#pragma region  D=150
		case 150:
			Cr = 265;
			Cor = 415;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33118;
			break;
#pragma endregion
#pragma region  D=160
		case 160:
			Cr = 345;
			Cor = 500;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33218;
			break;
#pragma endregion
#pragma region  D=190
		case 190:
			Cr = 502;
			Cor = 682;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32318;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=95
	case 95:
		switch (InOuterDiameter)
		{
#pragma region  D=130
		case 130:
			Cr = 102;
			Cor = 170;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 32919;
			break;
#pragma endregion
#pragma region  D=145
		case 145:
			Cr = 240;
			Cor = 390;
			e = 0.28;
			Y = 2.2;
			Yo = 1.2;
			stype = 33019;
			break;
#pragma endregion
#pragma region  D=160
		case 160:
			Cr = 312;
			Cor = 498;
			e = 0.39;
			Y = 1.5;
			Yo = 0.8;
			stype = 33119;
			break;
#pragma endregion
#pragma region  D=170
		case 170:
			Cr = 395;
			Cor = 568;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 33219;
			break;
#pragma endregion
#pragma region  D=200
		case 200:
			Cr = 540;
			Cor = 738;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32319;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=100
	case 100:
		switch (InOuterDiameter)
		{
#pragma region  D=140
		case 140:
			Cr = 135;
			Cor = 218;
			e = 0.33;
			Y = 1.8;
			Yo = 1;
			stype = 32920;
			break;
#pragma endregion
#pragma region  D=150
		case 150:
			Cr = 240;
			Cor = 390;
			e = 0.29;
			Y = 2.1;
			Yo = 1.2;
			stype = 33020;
			break;
#pragma endregion
#pragma region  D=165
		case 165:
			Cr = 322;
			Cor = 528;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 33120;
			break;
#pragma endregion
#pragma region  D=180
		case 180:
			Cr = 458;
			Cor = 665;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33220;
			break;
#pragma endregion
#pragma region  D=215
		case 215:
			Cr = 628;
			Cor = 872;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32320;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=105
	case 105:
		switch (InOuterDiameter)
		{
#pragma region  D=145
		case 145:
			Cr = 135;
			Cor = 225;
			e = 0.34;
			Y = 1.8;
			Yo = 1;
			stype = 32921;
			break;
#pragma endregion
#pragma region  D=160
		case 160:
			Cr = 270;
			Cor = 438;
			e = 0.28;
			Y = 2.1;
			Yo = 1.2;
			stype = 33021;
			break;
#pragma endregion
#pragma region  D=175
		case 175:
			Cr = 368;
			Cor = 608;
			e = 0.4;
			Y = 1.5;
			Yo = 0.8;
			stype = 33121;
			break;
#pragma endregion
#pragma region  D=190
		case 190:
			Cr = 298;
			Cor = 578;
			e = 0.42;
			Y = 1.4;
			Yo = 0.8;
			stype = 33221;
			break;
#pragma endregion
#pragma region  D=225
		case 225:
			Cr = 678;
			Cor = 945;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32321;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=110
	case 110:// 数据有误，检查;
		switch (InOuterDiameter)
		{
#pragma region  D=150
		case 150:
			Cr = 135;
			Cor = 232;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 32922;
			break;
#pragma endregion
#pragma region  D=170
		case 170:
			Cr = 302;
			Cor = 502;
			e = 0.29;
			Y = 2.1;
			Yo = 1.2;
			stype = 33022;
			break;
#pragma endregion
#pragma region  D=180
		case 180:
			Cr = 390;
			Cor = 638;
			e = 0.42;
			Y = 1.4;
			Yo = 0.8;
			stype = 33122;
			break;
#pragma endregion
#pragma region  D=200
		case 200:
			Cr = 450;
			Cor = 665;
			e = 0.42;
			Y = 1.4;
			Yo = 0.8;
			stype = 33222;
			break;
#pragma endregion
#pragma region  D=240
		case 240:
			Cr = 760;
			Cor = 1060;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32322;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=120
	case 120:
		switch (InOuterDiameter)
		{
#pragma region  D=165
		case 165:
			Cr = 180;
			Cor = 318;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32924;
			break;
#pragma endregion
#pragma region  D=180
		case 180:
			Cr = 312;
			Cor = 535;
			e = 0.31;
			Y = 2;
			Yo = 1.1;
			stype = 33024;
			break;
#pragma endregion
#pragma region  D=200
		case 200:
			Cr = 470;
			Cor = 778;
			e = 0.40;
			Y = 1.5;
			Yo = 0.8;
			stype = 33124;
			break;
#pragma endregion
#pragma region  D=215
		case 215:
			Cr = 500;
			Cor = 758;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32224;
			break;
#pragma endregion
#pragma region  D=260
		case 260:
			Cr = 865;
			Cor = 1230;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32324;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=130
	case 130:
		switch (InOuterDiameter)
		{
#pragma region  D=180
		case 180:
			Cr = 215;
			Cor = 380;
			e = 0.34;
			Y = 1.8;
			Yo = 1;
			stype = 32926;
			break;
#pragma endregion
#pragma region  D=200
		case 200:
			Cr = 418;
			Cor = 728;
			e = 0.34;
			Y = 1.8;
			Yo = 1;
			stype = 33026;
			break;
#pragma endregion
#pragma region  D=230
		case 230:
			Cr = 578;
			Cor = 888;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32226;
			break;
#pragma endregion
#pragma region  D=280
		case 280:
			Cr = 620;
			Cor = 805;
			e = 0.83;
			Y = 0.7;
			Yo = 0.4;
			stype = 31326;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=140
	case 140:
		switch (InOuterDiameter)
		{
#pragma region  D=190
		case 190:
			Cr = 218;
			Cor = 392;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 32928;
			break;
#pragma endregion
#pragma region  D=210
		case 210:
			Cr = 428;
			Cor = 755;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 33028;
			break;
#pragma endregion
#pragma region  D=250
		case 250:
			Cr = 675;
			Cor = 1050;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32228;
			break;
#pragma endregion
#pragma region  D=300
		case 300:
			Cr = 710;
			Cor = 928;
			e = 0.83;
			Y = 0.7;
			Yo = 0.4;
			stype = 31328;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=150
	case 150:
		switch (InOuterDiameter)
		{
#pragma region  D=210
		case 210:
			Cr = 272;
			Cor = 510;
			e = 0.33;
			Y = 1.8;
			Yo = 1;
			stype = 32930;
			break;
#pragma endregion
#pragma region  D=225
		case 225:
			Cr = 482;
			Cor = 875;
			e = 0.36;
			Y = 1.7;
			Yo = 0.9;
			stype = 33030;
			break;
#pragma endregion
#pragma region  D=270
		case 270:
			Cr = 755;
			Cor = 1180;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32230;
			break;
#pragma endregion
#pragma region  D=320
		case 320:
			Cr = 808;
			Cor = 1070;
			e = 0.83;
			Y = 0.7;
			Yo = 0.4;
			stype = 31330;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=160
	case 160:
		switch (InOuterDiameter)
		{
#pragma region  D=220
		case 220:
			Cr = 275;
			Cor = 525;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 32932;
			break;
#pragma endregion
#pragma region  D=240
		case 240:
			Cr = 440;
			Cor = 735;
			e = 0.46;
			Y = 1.3;
			Yo = 0.7;
			stype = 32032;
			break;
#pragma endregion
#pragma region  D=290
		case 290:
			Cr = 898;
			Cor = 1430;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32232;
			break;
#pragma endregion
#pragma region  D=340
		case 340:
			Cr = 920;
			Cor = 1190;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 30332;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=170
	case 170:
		switch (InOuterDiameter)
		{
#pragma region  D=230
		case 230:
			Cr = 295;
			Cor = 560;
			e = 0.38;
			Y = 1.6;
			Yo = 0.9;
			stype = 32934;
			break;
#pragma endregion
#pragma region  D=260
		case 260:
			Cr = 545;
			Cor = 920;
			e = 0.44;
			Y = 1.4;
			Yo = 0.7;
			stype = 32034;
			break;
#pragma endregion
#pragma region  D=310
		case 310:
			Cr = 1015;
			Cor = 1640;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32234;
			break;
#pragma endregion
#pragma region  D=360
		case 360:
			Cr = 1042;
			Cor = 1370;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 30334;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=180
	case 180:
		switch (InOuterDiameter)
		{
#pragma region  D=250
		case 250:
			Cr = 355;
			Cor = 708;
			e = 0.48;
			Y = 1.3;
			Yo = 0.7;
			stype = 32936;
			break;
#pragma endregion
#pragma region  D=280
		case 280:
			Cr = 670;
			Cor = 1150;
			e = 0.42;
			Y = 1.4;
			Yo = 0.8;
			stype = 32036;
			break;
#pragma endregion
#pragma region  D=320
		case 320:
			Cr = 1045;
			Cor = 1720;
			e = 0.45;
			Y = 1.3;
			Yo = 0.7;
			stype = 32236;
			break;
#pragma endregion
#pragma region  D=380
		case 380:
			Cr = 1142;
			Cor = 1500;
			e = 0.35;
			Y = 1.7;
			Yo = 1;
			stype = 30336;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=190
	case 190:
		switch (InOuterDiameter)
		{
#pragma region  D=260
		case 260:
			Cr = 378;
			Cor = 740;
			e = 0.48;
			Y = 1.3;
			Yo = 0.7;
			stype = 32938;
			break;
#pragma endregion
#pragma region  D=290
		case 290:
			Cr = 682;
			Cor = 1180;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32038;
			break;
#pragma endregion
#pragma region  D=340
		case 340:
			Cr = 1175;
			Cor = 1900;
			e = 0.44;
			Y = 1.4;
			Yo = 0.8;
			stype = 32238;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
#pragma region  d=200
	case 200:
		switch (InOuterDiameter)
		{
#pragma region  D=280
		case 280:
			Cr = 482;
			Cor = 950;
			e = 0.39;
			Y = 1.5;
			Yo = 0.8;
			stype = 32940;
			break;
#pragma endregion
#pragma region  D=310
		case 310:
			Cr = 818;
			Cor = 1420;
			e = 0.43;
			Y = 1.4;
			Yo = 0.8;
			stype = 32040;
			break;
#pragma endregion
#pragma region  D=360
		case 360:
			Cr = 1382;
			Cor = 2180;
			e = 0.41;
			Y = 1.5;
			Yo = 0.8;
			stype = 32240;
			break;
#pragma endregion
		default:
			stype = -1;
			
			break;
		}
		break;
#pragma endregion
	default:
		stype = -1;
		
		break;
	}
	Cr = Cr * 1000;
	Cor = Cor * 1000;
}

void CBearingTAN::EquivalentLoadP(double Fa, double Fr)
{
	switch (mBearingType) 
	{
		//深沟球轴承
		case DeepGrooveBallBearing:
			// NOTE: this branch used to be empty, so P/P0 kept their uninitialized
			// values and the life was garbage (often Inf). The UI does not provide
			// e/Y for deep groove ball bearings, so use the radial load only.
			P = Fr;
			P0 = Fr;
			break;
		//滚针轴承
		case NeedleBearing:
			P = Fr;
			P0 = Fr;
			break;
		//圆锥滚子轴承
		case TaperedRollerBearing:
			if (Fa / Fr <= e)  //计算当量动载荷P
				P = Fr;
			else
			{
				P = (0.4 * Fr + Y * Fa);
			}
			//径向当量静载荷
			P0 = (0.5 * Fr + Yo * Fa);
			if (P0 < Fr)  //计算当量静载荷Po
			{
				P0 = Fr;
			}
			break;
		// 角接触球轴承 测试
		case AngularContactBallBearing:
			if (Fa / Fr <= e)
				P = Fr;
			else
				P = 0.44 * Fr + Y * Fa;

			P0 = 0.5 * Fr + 0.46 * Fa;
			if (P0 < Fr)
				P0 = Fr;
			break; 
	}


}

void CBearingTAN::CalFh()
{
	fh = fn * ft*Cr / (fm*fd*P);
}

double CBearingTAN::CalLifeByFh()
{	
	// The fh-Lh curve is defined on fh in [0.617, 4.90] (i.e. L10h in [100, 100000] h).
	// The old code extrapolated the cubic spline directly: for fh < 0.617 (overloaded
	// bearing, life below 100 h) the extrapolated value becomes NEGATIVE, which is why
	// the UI displayed negative bearing life. Clamp fh into the curve domain instead,
	// so the life is always a positive, bounded value.
	if (mFh2LhCurve.mpVarList == nullptr || mFh2LhCurve.mPointCount <= 0)
		return -1.0;
	if (!std::isfinite(fh) || fh <= 0.0)
		return -1.0; // invalid input: rpm outside the table range, or non-positive load/Cr
	const double tmpFhMin = mFh2LhCurve.mpVarList[0];
	const double tmpFhMax = mFh2LhCurve.mpVarList[mFh2LhCurve.mPointCount - 1];
	double tmpFh = fh;
	if (tmpFh < tmpFhMin) tmpFh = tmpFhMin;
	if (tmpFh > tmpFhMax) tmpFh = tmpFhMax;
	return mFh2LhCurve.CalInterpValueWithExtend(tmpFh, mFh2LhCurve);
}

void CBearingTAN::Dynamic_Load(double InBearingLife,double InRPM)
{
	fh = fhSet(static_cast<int>(InBearingLife));
	fn = fnSet(InRPM);
	C = ((fh*fm*fd) / (fn*ft))*P;

}

void CBearingTAN::Static_Load()
{
	So = 2;
	Co = So*P0;	
}

void CBearingTAN::BearingBasicLifeL10(double Fa, double Fr)
{
	//计算出P
	EquivalentLoadP(Fa, Fr);
	//Dynamic_Load();
	Static_Load();
	CalAdditionalAxisForce(Fr);
	//圆锥滚子轴承
	L10 = (pow(10.0, 6.0) / (60.0 * n))*pow(Cr / P, epsilon);
	//深沟球轴承
//	L10 = (pow(10.0, 6.0) / (60.0 * n))*pow(Cr / P, 3.0);
}

//double CBearingTAN::Ex_Fd(double Fr)
//{
//	Fd = Fr / (2 * Y);
//	return Fd;
//}
double CBearingTAN::GetBearingRealLife(double InFa, double InFr)
{
	EquivalentLoadP(InFa, InFr);
	fnSet(n);
	CalFh();
	RealLife = CalLifeByFh();
	return RealLife;
}

void CBearingTAN::DefineTaperedRollerLocationPara(int InAssembleFaceType,//安装面定位面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
	double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离
	int InBearingForcePointFaceType,//轴承作用点对应的轴承面类型，1-外圈侧壁,左侧，-1-内圈侧壁，右侧，0-作用点
	double InBearingForcePointDist,//轴承作用点距离距轴承作用点参考面距离	
	int InBearingAssembleOrient,//轴承轴线方向 0-轴承轴线方向与轴一致，接触角指向远离轴左侧方向，1-轴承轴线方向与轴相反，接触角指向靠近轴左侧方向);)
	double InBearingWidth)//轴承宽度
{
	AssembleFaceType = InAssembleFaceType;
	AssebleOrientation = InBearingAssembleOrient;
	//确定安装定义面和轴承作用点是否定义面一致
	//如果一致
	if (AssembleFaceType == InBearingForcePointFaceType)
	{

		AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
		AssembleForcePointDistPara = InBearingForcePointDist;
	}
	//如果不一致,将安装参考面调整到作用点位置
	else
	{
		if (AssembleFaceType == AssembleMiddleBaseNotation)
		{
			AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
		}
		else if (InBearingForcePointFaceType == BearingForcePointToRightFace)
		{
			AssembleDist = InDistBetweenAssembleEdgeToShaftLeft + AssebleOrientation * (InBearingWidth - InBearingForcePointDist);
		}
		else
		{
			AssembleDist = InDistBetweenAssembleEdgeToShaftLeft - AssebleOrientation * (InBearingWidth - InBearingForcePointDist);
		}
		AssembleForcePointDistPara = 0.0;
		AssembleFaceType = AssembleMiddleBaseNotation;
	}
	Width = InBearingWidth;
}
void CBearingTAN::DefineRadialBearingLocationPara(int InAssembleFaceType,//安装面定位面类型，1-左侧，-1-右侧，0-中间
	double InDistBetweenAssembleEdgeToShaftLeft,//轴承安装面距离轴左端距离 		
	int InBearingAssembleOrient,//轴承安装朝向，1-正向，-1反向	
	double InBearingWidth)//轴承宽度
{
	AssembleFaceType = InAssembleFaceType;
	AssebleOrientation = InBearingAssembleOrient;
	if (AssembleFaceType == AssembleMiddleBaseNotation)
	{
		AssembleDist = 0.0;
		AssembleForcePointDistPara = 0.0;
	}
	else if (AssembleFaceType * AssebleOrientation == 1)
	{
		AssembleFaceType = AssembleLeftBaseNotation;
		AssebleOrientation = BearingExForceToRightNotation;
		AssembleForcePointDistPara = InBearingWidth / 2;
	}
	else
	{
		AssembleFaceType = AssembleRightBaseNotation;
		AssebleOrientation = BearingExForceToRightNotation;
		AssembleForcePointDistPara = InBearingWidth / 2;
	}
	AssembleDist = InDistBetweenAssembleEdgeToShaftLeft;
	Width = InBearingWidth;
}

void CBearingTAN::DefineLocationPara(double* InAssembleData)
{

	if (mBearingType == DeepGrooveBallBearing ||
		mBearingType == NeedleBearing)//如果是对称结构
	{
		DefineRadialBearingLocationPara(
			static_cast<int>(InAssembleData[0]),
			InAssembleData[1],
			static_cast<int>(InAssembleData[4]),
			InAssembleData[5]);
	}
	else
	{
		DefineTaperedRollerLocationPara(static_cast<int>(InAssembleData[0]),
			InAssembleData[1],
			static_cast<int>(InAssembleData[2]),
			InAssembleData[3],
			static_cast<int>(InAssembleData[4]),
			InAssembleData[5]);
	}
}
double CBearingTAN::CalAdditionalAxisForce(double InFr)
{
	if (mBearingType==TaperedRollerBearing)
	{
		Fa_ex = InFr / (2 * Y)*AssebleOrientation;
	}
	else
	{
		Fa_ex = 0;
	}
	return Fa_ex;
}
double CBearingTAN::CalRealAxisForce(double InTotalShaftAxisForce, int InBearForceBearingCount)
{
	if (mBearingType==DeepGrooveBallBearing)
	{
		Fa = -InTotalShaftAxisForce/ InBearForceBearingCount;
	}
	//表明带有附加轴向力
	else if (abs(Fa_ex)>DefaultTolerance)
	{
		if (NSTanBasiceFunction::Fsign(InTotalShaftAxisForce) != NSTanBasiceFunction::Fsign(Fa_ex))//总体运动趋势方向与轴承力方向相反，轴承被压紧
		{
			Fa = Fa_ex -InTotalShaftAxisForce / InBearForceBearingCount;
		}
		else
		{
			Fa = Fa_ex;
		}
	}
	else
	{
		Fa = 0.0;
	}

	return Fa;
}
int CBearingTAN::JudgeBearingIsAxisConstrain(double InTotalAxisForce)
{
	if (mBearingType == DeepGrooveBallBearing)
	{
		return 1;
	}
	else if (mBearingType == NeedleBearing)
	{
		return 0;
	}
	else if (mBearingType == TaperedRollerBearing|| mBearingType == AngularContactBallBearing)
	{
		if (NSTanBasiceFunction::Fsign(InTotalAxisForce) != NSTanBasiceFunction::Fsign(Fa_ex))
		{
			if (abs(InTotalAxisForce)>DefaultTolerance)
			{
				return 1;
			}
			else
			{
				return 0;
			}
			
		}
		else
		{
			return 0;
		}
	}
}

void CBearingTAN::InitialLifeCurve()
{
	mLh2FhCurve.mPointCount = 210;
	mLh2FhCurve.mPointDimension = 1;
	mLh2FhCurve.mpVarList = &(NSBearingStaticArrary::sLhListInBook[0]);
	mLh2FhCurve.mpSurfCoef = &(NSBearingStaticArrary::sLh2FhCoef[0]);
	mFh2LhCurve.mPointCount = 210;
	mFh2LhCurve.mPointDimension = 1;
	mFh2LhCurve.mpVarList = &(NSBearingStaticArrary::sFhListInBook[0]);
	mFh2LhCurve.mpSurfCoef = &(NSBearingStaticArrary::sFh2LhCoef[0]);
}

void CBearingTAN::DefineDeepGrooveBallBearing(double InRPM,  double InCr, double InC0r)
{
	n = InRPM;
	Cr = InCr;
	Cor = InC0r;
	epsilon = mBallBearingCoef;
	mBearingType = DeepGrooveBallBearing;
	
}
void CBearingTAN::DefineNeedleBearing(double Inn1, double InCr1, double InC0r1)
{
	n = Inn1;
	Cr = InCr1;
	Cor = InC0r1;
	epsilon = mRollerbearingCoef;
	mBearingType = NeedleBearing;
}
void CBearingTAN::DefineAngularContactBallBearing(double n1, double Cr1, double C0r1, double tmpe, double tmpY, double tmpY1, double tmpY2)
{
	n = n1; Cr = Cr1; Cor = C0r1; epsilon = mBallBearingCoef; e = tmpe;  Y = tmpY; Y1 = tmpY1; Y2 = tmpY2;
	mBearingType = AngularContactBallBearing;
}
////圆锥滚子轴承实现1
//void CBearingTAN::bevel_ex1(double n1, int L1, int d1, int D1)
//{
//	n = n1; L = L1; d = d1; InOuterDiameter = D1;
//	
//}
//圆锥滚子轴承实现2
void CBearingTAN::DefineTaperedRollerBearing(double n1, double Cr1, double Cor1, double e1, double Y1, double Yo1 )
{
	n = n1; e = e1; Y = Y1; Cr = Cr1; Yo = Yo1; Cor = Cor1; epsilon = mRollerbearingCoef;
	mBearingType = TaperedRollerBearing;
}

void CBearingTAN::DefineRollingBearing(int InBearingType, double* InBearingData)
{
	switch (InBearingType)
	{
	case (DeepGrooveBallBearing):
		DefineDeepGrooveBallBearing(InBearingData[0], InBearingData[1], InBearingData[2]);
		break;
	case (NeedleBearing):
		DefineNeedleBearing(InBearingData[0], InBearingData[1], InBearingData[2]);
		break;
	case (TaperedRollerBearing):
		DefineTaperedRollerBearing(InBearingData[0], //额定转速
			InBearingData[1], //基本额定动载荷  Cr1
			InBearingData[2], //基本额定静载荷 Cor1
			InBearingData[3], //计算系数  e1
			InBearingData[4], //计算系数 Y1
			InBearingData[5] //计算系数 Yo1
		);
		break;
	case(AngularContactBallBearing):
		DefineAngularContactBallBearing(InBearingData[0], InBearingData[1], InBearingData[2], InBearingData[3], InBearingData[4], InBearingData[5], InBearingData[6]);
		break;
	}
		
}
//（轴承类型，[n,fa,fr,life])
void CBearingTAN::CalAndRecommedBearingLife(int InBearingType, double* InBearingData)
{
	MYSQL* mysql = mysql_init(NULL);// 初始化连接环境
	mysql = mysql_real_connect(mysql, "localhost", "root", "jay", "bearings", 3729, NULL, 0);// 连接数据库服务器

	// 动态获取数据表名
	std::string tableName;
	switch (InBearingType)
	{
	case DeepGrooveBallBearing: tableName = "deepgrooveball"; break;
	case NeedleBearing: tableName = "needlebearing"; break; //目前只临时添加了一个滚针轴承数据表到数据库中（8-358）
	case TaperedRollerBearing: tableName = "taperedrollerbearingsinsinglerows"; break;
	case AngularContactBallBearing: tableName = "angularcontactballbearingsingle"; break;
	default: tableName = "bearing_default";
	}

	// 执行查询
	std::string query = "SELECT * FROM " + tableName;
	mysql_query(mysql, query.c_str());

	// 获取结果集
	MYSQL_RES* result = mysql_store_result(mysql);
	if (!result) {
		std::cerr << "获取结果集失败: " << mysql_error(mysql) << std::endl;
		mysql_close(mysql);
		return;
	}
	std::vector<BearingData> bearingList;//存放轴承数据,记得将数据库中的Cr和C0r的单位转换为N(默认是以kN为单位)
	MYSQL_ROW row;
	while ((row = mysql_fetch_row(result))) {
		BearingData bearing;
		bearing.code = row[0] ? row[0] : "";
		bearing.d = row[1] ? std::stod(row[1]) : 0.0;
		bearing.D = row[2] ? std::stod(row[2]) : 0.0;
		bearing.B = row[3] ? std::stod(row[3]) : 0.0;
		bearing.Cr = row[4] ? std::stod(row[4]) * 1000.0 : 0.0;
		bearing.C0r = row[5] ? std::stod(row[5]) * 1000.0 : 0.0;
		if (InBearingType == TaperedRollerBearing)
		{
			bearing.e = row[6] ? std::stod(row[6]) : 0.0;
			bearing.Y = row[7] ? std::stod(row[7]) : 0.0;
			bearing.Yo = row[8] ? std::stod(row[8]) : 0.0;
		}

		bearingList.push_back(bearing);
	}
	std::vector<BearingData> overLifeList;//存放超寿命轴承数据

	// 根据类型条件，遍历数据库进行寿命计算 
	if (InBearingType == DeepGrooveBallBearing || InBearingType == NeedleBearing)
	{
		for (int i = 0; i < bearingList.size(); i++)
		{
			CBearingTAN testBearing(InBearingType, new double[3] {InBearingData[0], bearingList[i].Cr, bearingList[i].C0r});
			bearingList[i].life = testBearing.GetBearingRealLife(InBearingData[1], InBearingData[2]); //传入Fa，Fr调用函数得到轴承寿命
			if (bearingList[i].life > InBearingData[3])
			{
				overLifeList.push_back(bearingList[i]);
			}
		}
	}
	else if (InBearingType == TaperedRollerBearing)
	{
		for (int i = 0; i < bearingList.size(); i++)
		{
			CBearingTAN testBearing(InBearingType, new double[6] {InBearingData[0], bearingList[i].Cr, bearingList[i].C0r, bearingList[i].e, bearingList[i].Y, bearingList[i].Yo});
			bearingList[i].life = testBearing.GetBearingRealLife(InBearingData[1], InBearingData[2]); //传入Fa，Fr调用函数得到轴承寿命
			if (bearingList[i].life > InBearingData[3])
			{
				overLifeList.push_back(bearingList[i]);
			}
		}
	}
	else if (InBearingType == AngularContactBallBearing)
	{
		for (int i = 0; i < bearingList.size(); i++)
		{
			double FaOverC0r = InBearingData[1] / bearingList[i].C0r;
			double e, Y, Y1, Y2;
			CBearingTAN TestBearing;
			if (TestBearing.GetLoadCoeff(FaOverC0r, e, Y, Y1, Y2))
			{
				CBearingTAN testBearing(InBearingType, new double[7] {InBearingData[0], bearingList[i].Cr, bearingList[i].C0r, e, Y, Y1, Y2});
				bearingList[i].life = testBearing.GetBearingRealLife(InBearingData[1], InBearingData[2]); //传入Fa，Fr调用函数得到轴承寿命
				if (bearingList[i].life > InBearingData[3])
				{
					overLifeList.push_back(bearingList[i]);
				}
			}
		}
	}
	else
	{
		std::cerr << "这里对所有需要的轴承进行寿命计算。可新建一个数据表格，包含所需的所有轴承，并且统一表头" << std::endl;
	}

	//std::vector<BearingData> topClosestList;//存放寿命最接近目标寿命的轴承数据
	if (overLifeList.size() == 0)
	{
		std::cout << "未发现超寿命轴承" << std::endl;
		return;
	}

	else
	{
		// 按与 InBearingData[3] 的寿命差进行排序
		std::sort(overLifeList.begin(), overLifeList.end(),
			[targetLife = InBearingData[3]](const BearingData& a, const BearingData& b) {
				//return std::abs(a.life - targetLife) < std::abs(b.life - targetLife);
				if (a.d == b.d)
					return a.B < b.B;  // 同直径，选宽度小的
				return a.d < b.d;
			});

		// 拷贝前十个（如果不足10个就拷贝全部）
		int count = (overLifeList.size() < 2000) ? overLifeList.size() : 2000;
		mBearingDataVec.assign(overLifeList.begin(), overLifeList.begin() + count);

		// 打印确认
		std::cout << "最接近目标寿命的前 " << count << " 个轴承：" << std::endl;
		for (const auto& b : mBearingDataVec) {
			std::cout << ", Name: " << b.code
				<< ", Life: " << b.life << ", Cr: " << b.Cr << ", C0r: " << b.C0r << std::endl;
		}

	}

}
std::vector<CBearingTAN::BearingData> CBearingTAN::ObtainBearingDataVec(int InBearingType)
{
	std::vector<BearingData> resultBearingData;
	MYSQL* mysql = mysql_init(NULL);// 初始化连接环境
	mysql = mysql_real_connect(mysql, "localhost", "root", "jay", "bearings", 3729, NULL, 0);// 连接数据库服务器
	//判断连接是否成功
	if (mysql == NULL) {
		std::cerr << "数据库连接失败: " << mysql_error(mysql) << std::endl;
		return resultBearingData;
	}
	// 动态获取数据表名
	std::string tableName;
	switch (InBearingType)
	{
	case DeepGrooveBallBearing: tableName = "deepgrooveball"; break;
	case NeedleBearing: tableName = "needlebearing"; break; //目前只临时添加了一个滚针轴承数据表到数据库中（8-358）
	case TaperedRollerBearing: tableName = "taperedrollerbearingsinsinglerows"; break;
	case AngularContactBallBearing: tableName = "angularcontactballbearingsingle"; break;
	default: tableName = "bearing_default";
	}

	// 执行查询
	std::string query = "SELECT * FROM " + tableName;
	mysql_query(mysql, query.c_str());

	// 获取结果集
	MYSQL_RES* result = mysql_store_result(mysql);
	if (!result) {
		std::cerr << "获取结果集失败: " << mysql_error(mysql) << std::endl;
		mysql_close(mysql);
		return resultBearingData;
	}
	MYSQL_ROW row;
	switch (InBearingType)
	{
		case NeedleBearing:
			while ((row = mysql_fetch_row(result))) {
				BearingData bearing;
				bearing.code = row[0] ? row[0] : "";
				bearing.d = row[1] ? std::stod(row[1]) : 0.0; //FW
				bearing.D = row[2] ? std::stod(row[2]) : 0.0; //EW
				bearing.B = row[3] ? std::stod(row[3]) : 0.0; //Bc
				bearing.Cr = row[4] ? std::stod(row[4]) * 1000.0 : 0.0;
				bearing.C0r = row[5] ? std::stod(row[5]) * 1000.0 : 0.0;
				resultBearingData.push_back(bearing);

				std::sort(resultBearingData.begin(), resultBearingData.end(),
					[](const BearingData& a, const BearingData& b) {
						if (a.d == b.d)
							return a.B < b.B;  // 同直径，选宽度小的
						return a.d < b.d;      // 先按内径升序
					});
			}
			break;
		case TaperedRollerBearing:
			while ((row = mysql_fetch_row(result))) {
				BearingData bearing;
				bearing.code = row[0] ? row[0] : "";
				bearing.d = row[1] ? std::stod(row[1]) : 0.0;
				bearing.D = row[2] ? std::stod(row[2]) : 0.0;
				bearing.B = row[3] ? std::stod(row[3]) : 0.0;
				bearing.Cr = row[4] ? std::stod(row[4]) * 1000.0 : 0.0;
				bearing.C0r = row[5] ? std::stod(row[5]) * 1000.0 : 0.0;
				bearing.e = row[6] ? std::stod(row[6]) : 0.0;
				bearing.Y = row[7] ? std::stod(row[7]) : 0.0;
				bearing.Yo = row[8] ? std::stod(row[8]) : 0.0;
				bearing.a = row[9] ? std::stod(row[9]) : 0.0;
				bearing.T = row[10] ? std::stod(row[10]) : 0.0;
				bearing.C = row[11] ? std::stod(row[11]) : 0.0;
				resultBearingData.push_back(bearing);

				std::sort(resultBearingData.begin(), resultBearingData.end(),
					[](const BearingData& a, const BearingData& b) {
						if (a.d == b.d)
							return a.T < b.T;  // 同直径，选宽度小的
						return a.d < b.d;      // 先按内径升序
					});
			}
			break;
		case AngularContactBallBearing:
			while ((row = mysql_fetch_row(result))) {
				BearingData bearing;
				bearing.code = row[0] ? row[0] : "";
				bearing.d = row[1] ? std::stod(row[1]) : 0.0;
				bearing.D = row[2] ? std::stod(row[2]) : 0.0;
				bearing.B = row[3] ? std::stod(row[3]) : 0.0;
				bearing.Cr = row[4] ? std::stod(row[4]) * 1000.0 : 0.0;
				bearing.C0r = row[5] ? std::stod(row[5]) * 1000.0 : 0.0;
				bearing.a = row[6] ? std::stod(row[6]) : 0.0;
				bearing.contactAngle = 0.2618; //约等于15度

				//bearing.d = row[0] ? std::stod(row[0]) : 0.0;
				//bearing.D = row[1] ? std::stod(row[1]) : 0.0;
				//bearing.B = row[2] ? std::stod(row[2]) : 0.0;
				//bearing.Cr = row[3] ? std::stod(row[3]) : 0.0;
				//bearing.C0r = row[4] ? std::stod(row[4]) : 0.0;


				//bearing.contactAngle = row[8] ? std::stod(row[8]) : 0.0;
				//bearing.contactAngle = bearing.contactAngle * 180.0 / PI; // 将弧度转换为角度
				resultBearingData.push_back(bearing);

				std::sort(resultBearingData.begin(), resultBearingData.end(),
					[](const BearingData& a, const BearingData& b) {
						if (a.d == b.d)
							return a.B < b.B;  // 同直径，选宽度小的
						return a.d < b.d;      // 先按内径升序
					});
			}
			break;
		default:	
			break;
	}
	return resultBearingData;
}
double CBearingTAN::ObtainDistFromBearingLocatingSurfaceToReferenceSurface(int InBearingLocatingSurface, double InBearingA,  //InBearingA:轴承作用点距离距轴承作用点参考面距离
	double InBearingWidth,
	double tmpInLength) 	//tmpInLength:轴承左端面距离基准的距离
{
	double dist = 0.0;
	if (InBearingLocatingSurface == -1) //内圈侧壁，右侧（以轴承右侧为参考面）
	{
		dist = tmpInLength + InBearingWidth;
	}
	else if (InBearingLocatingSurface == 1)//外圈侧壁,左侧
	{
		dist = tmpInLength;
	}
	else if (InBearingLocatingSurface == 0)//0-作用点，先当作作用点，而不是中间位置@
	{
		dist = tmpInLength + InBearingA;
	}
	
	return dist;
}
bool CBearingTAN::GetLoadCoeff(double FaOverC0r,
	double& e, double& Y,
	double& Y1, double& Y2)
{
	// 边界处理：小于最小值，取第一个点
	if (FaOverC0r <= coeffTable.front().FaOverC0r)
	{
		const auto& data = coeffTable.front();
		e = data.e;
		Y = data.Y;
		Y1 = data.Y1;
		Y2 = data.Y2;
		return true;
	}
	// 大于最大值，取最后一个点
	if (FaOverC0r >= coeffTable.back().FaOverC0r)
	{
		const auto& data = coeffTable.back();
		e = data.e;
		Y = data.Y;
		Y1 = data.Y1;
		Y2 = data.Y2;
		return true;
	}

	// 查找区间：找到第一个大于FaOverC0r的点
	auto it = std::upper_bound(coeffTable.begin(), coeffTable.end(), FaOverC0r,
		[](double val, const BearingLoadCoeff& elem) {
			return val < elem.FaOverC0r;
		});

	// 前后两个点
	const auto& data1 = *(it - 1);
	const auto& data2 = *it;

	// 计算插值比例
	double t = (FaOverC0r - data1.FaOverC0r) /
		(data2.FaOverC0r - data1.FaOverC0r);

	// 线性插值计算所有系数
	e = data1.e + t * (data2.e - data1.e);
	Y = data1.Y + t * (data2.Y - data1.Y);
	Y1 = data1.Y1 + t * (data2.Y1 - data1.Y1);
	Y2 = data1.Y2 + t * (data2.Y2 - data1.Y2);

	return true;
}
//圆锥滚子轴承通用实现
//void CBearingTAN::bevel_ex(int para_method, double para[] )
//{
//	if (para_method == 0)
//	{
//		n = para[0]; 
//		L = round(para[1]);
//		d = round(para[2]);
//		InOuterDiameter = round(para[3]);
//		Para_switch();
//	}
//	if (para_method == 1)
//	{
//		n = para[0];
//		L = round(para[1]);
//		e = para[2]; 
//		Y = para[3];
//		Cr = para[4];
//		Yo = para[5];
//		Cor = para[6];
//	}
//}
