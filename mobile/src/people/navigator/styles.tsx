import React from 'react';
import { StyleSheet, Platform } from 'react-native';

const peopleScreenNavigatorStyles = StyleSheet.create({
    tabBar: {
        height: 60,
        paddingTop: 20,
        backgroundColor: '#2FAFCC',
        justifyContent: 'center'
    },
    tabBarIndicator: {
        backgroundColor: 'rgba(0, 0, 0, 0.4)',
    },
    tabBarLabel: {
        color: '#FFF',
        fontSize: 13,
        lineHeight: 15,
        textAlign: 'center',
        fontFamily: 'CenturyGothic'
    }
});


export default peopleScreenNavigatorStyles;
