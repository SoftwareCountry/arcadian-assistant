import React from 'react';
import { StyleSheet, Platform } from 'react-native';

const topTabBarStyles = StyleSheet.create({
    tabBar: {
        height: Platform.OS === 'ios' ? 50 : 50,
        backgroundColor: '#2FAFCC',
    },
    tabBarIndicator: {
        backgroundColor: 'transparent',
    },
    tabBarLabel: {
        color: '#FFFFFF',
        fontSize: 13,
        lineHeight: 15,
        textAlign: 'center'
    },
    tabImages: {
        width: 30, 
        height: 26 
    }
});


export default topTabBarStyles;
