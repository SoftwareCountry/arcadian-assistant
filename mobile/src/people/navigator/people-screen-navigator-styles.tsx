import React from 'react';
import { StyleSheet, Platform } from 'react-native';

const peopleScreenNavigatorStyles = StyleSheet.create({
    tabBar: {
        height: 50,
        backgroundColor: '#2FAFCC',
    },
    tabBarIndicator: {
        backgroundColor: 'transparent',
    },
    tabBarLabel: {
        color: '#FFF',
        fontSize: 13,
        lineHeight: 15,
        textAlign: 'center'
    }
});


export default peopleScreenNavigatorStyles;
