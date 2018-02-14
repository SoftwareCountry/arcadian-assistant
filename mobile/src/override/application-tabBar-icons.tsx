import React, { Component } from 'react';
import { createIconSet } from 'react-native-vector-icons';

const glyphMap = {
    'calendar': 0xE001,
    'feeds': 0xE002,
    'helpdesk': 0xE003,
    'people': 0xE004,
    'profile': 0xE005
};
export const ApplicationTabbarIcons = createIconSet(glyphMap, 'aa-iconfont');

