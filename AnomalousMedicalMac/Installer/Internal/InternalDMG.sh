#!/bin/sh

# BuildBundle.sh
# 
#
# Created by Andrew Piper on 1/18/11.
# Copyright 2011 Anomalous Software. All rights reserved.

THIS_FOLDER=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

pushd $THIS_FOLDER

rm -rf ../../bin/Release/Anomalous\ Medical.app
mv ../../bin/Release/AnomalousMedicalMac.app ../../bin/Release/Anomalous\ Medical.app

codesign -s "Developer ID Application: ANOMALOUS MEDICAL, LLC (DVCRZEJ9QF)" ../../bin/Release/Anomalous\ Medical.app --deep

sh ../MakeDMG.sh "Anomalous Medical" "../../bin/Release" "Anomalous Medical" "../Layout" "../../../AnomalousMedical/Installer/License"

rm AnomalousMedical.dmg

mv Anomalous\ Medical.dmg AnomalousMedical.dmg

popd