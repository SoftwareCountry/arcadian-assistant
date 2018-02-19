import React, { Component } from 'react';
import { createIconSet } from 'react-native-vector-icons';

const glyphMap = {
    'birthday': 0xE001,
    'calendar': 0xE002,
    'dayoff': 0xE003,
    'envelope': 0xE004,
    'feeds': 0xE005,
    'handshake': 0xE006,
    'helpdesk': 0xE007,
    'office': 0xE008,
    'org_structure': 0xE009,
    'people': 0xE00A,
    'phone': 0xE00B,
    'profile': 0xE00C,
    'search': 0xE00D,
    'settings': 0xE00E,
    'sick_leave': 0xE00F,
    'vacation': 0xE010
};
export const ApplicationIcon = createIconSet(glyphMap, 'aa-iconfont');

