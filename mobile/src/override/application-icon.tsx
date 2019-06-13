import React from 'react';
import { createIconSet } from 'react-native-vector-icons';

const glyphMap = {
    'approve-tick': 0xE011,
    'birthday': 0xE001,
    'calendar': 0xE002,
    'dayoff': 0xE004,
    'employees': 0xE012,
    'envelope': 0xE005,
    'feeds': 0xE006,
    'handshake': 0xE007,
    'help_desk': 0xE008,
    'office': 0xE009,
    'org_structure': 0xE00A,
    'people': 0xE00B,
    'phone': 0xE00C,
    'profile': 0xE00D,
    'reject-cross': 0xE003,
    'search': 0xE00E,
    'settings': 0xE00F,
    'sick_leave': 0xE010,
    'vacation': 0xE013
};
export const ApplicationIcon = createIconSet(glyphMap, 'aa-iconfont');

const glyphMapBold = {
    'calendar': 0xE001,
    'feeds': 0xE002,
    'help_desk': 0xE003,
    'people': 0xE004,
    'profile': 0xE005
};

export const ApplicationIconBold = createIconSet(glyphMapBold, 'aa-iconfont-bold');
