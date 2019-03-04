import React from 'react';
import { StyleSheet } from 'react-native';
import Style from '../../layout/style';

const peopleScreenNavigatorStyles = StyleSheet.create({
    tabBar: {
        height: 40,
        paddingTop: 0,
        backgroundColor: Style.color.base,
        justifyContent: 'center'
    },
    tabBarIndicator: {
        backgroundColor: 'rgba(0, 0, 0, 0.4)',
    },
    tabBarLabel: {
        color: Style.color.white,
        fontSize: 13,
        lineHeight: 15,
        textAlign: 'center',
        fontFamily: 'CenturyGothic'
    }
});


export default peopleScreenNavigatorStyles;
