TOP=../..

include $(TOP)/configure/CONFIG
#----------------------------------------
#  ADD MACRO DEFINITIONS AFTER THIS LINE
#=============================

#=============================
# Build the IOC application

PROD_IOC = simIOC
# simIOC.dbd will be created and installed
DBD += simIOC.dbd

# simIOC.dbd will be made up from these files:
simIOC_DBD += base.dbd

# Include dbd files from all support applications:
#simIOC_DBD += xxx.dbd

# Add all the support libraries needed by this IOC
#simIOC_LIBS += xxx

# simIOC_registerRecordDeviceDriver.cpp derives from simIOC.dbd
simIOC_SRCS += simIOC_registerRecordDeviceDriver.cpp

# Build the main IOC entry point on workstation OSs.
simIOC_SRCS_DEFAULT += simIOCMain.cpp
simIOC_SRCS_vxWorks += -nil-

# Add support from base/src/vxWorks if needed
#simIOC_OBJS_vxWorks += $(EPICS_BASE_BIN)/vxComLibrary

# Finally link to the EPICS Base libraries
simIOC_LIBS += $(EPICS_BASE_IOC_LIBS)

#===========================

include $(TOP)/configure/RULES
#----------------------------------------
#  ADD RULES AFTER THIS LINE

