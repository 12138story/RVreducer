#pragma once

// 内部实现宏：将当前齿轮副成员绑定为局部引用；只在计算函数体中展开。
// 保留原有别名及引用目标，不改变参数值、对象布局或 DLL 接口。
#define _CiteGearPara \
int& z1 = mPinion.Z;\
int& z2 = mGear.Z; \
double& z_n1 = mPinion.zn; \
double& z_n2 = mGear.zn;\
double& d_a1 = mPinion.da;\
double& d_a2 = mGear.da;\
double& d_b1 = mPinion.db;\
double& d_b2 = mGear.db;\
double& d_1 = mPinion.d;\
double& d_2 = mGear.d;\
double& d_m1 = mPinion.dm;\
double& d_m2 = mGear.dm;\
double& d_Fa1 = mPinion.d_Fa;\
double& d_Fa2 = mGear.d_Fa;\
double& d_Nf1 = mPinion.d_Nf;\
double& d_Nf2 = mGear.d_Nf;\
double& d_Ff1 = mPinion.d_Ff;\
double& d_Ff2 = mGear.d_Ff;\
double& d_Na1 = mPinion.d_Na;\
double& d_Na2 = mGear.d_Na;\
double& x1 = mPinion.x;\
double& x2 = mGear.x;\
double& h_fP1=mPinion.h_fP;\
double& h_fP2 = mGear.h_fP;\
double& q1=mPinion.DidmRatio;\
double& q2=mGear.DidmRatio;\
double& rho1=mPinion.Density;\
double& rho2=mGear.Density;\

// 保留宏末行的续行符及其后的空行，使展开边界与原文件一致。
