import React, { Component } from 'react';
import { createIconSet } from 'react-native-vector-icons';

const glyphMap = {
    'calendar': 0xE002,
    'feeds': 0xE005,
    'helpdesk': 0xE007,
    'people': 0xE00A,
    'profile': 0xE00C
};
export const ApplicationTabbarIcons = createIconSet(glyphMap, 'aa-iconfont');

