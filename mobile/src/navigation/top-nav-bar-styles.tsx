import React from 'react';
import { StyleSheet, Platform } from 'react-native';

const topNavBarStyles = StyleSheet.create({
    container: {
        marginTop: 0,
        height: 50,
        backgroundColor: '#2FAFCC',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
    },
    iconContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
    },
    titleContainer: {
        flex: 8,
        alignItems: 'center',
    },
    navTitle: {
        color: 'white', 
        textAlign: 'center'
    },
});

export const logoutStyles = StyleSheet.create({
    logoutContainer: {
        alignItems: 'flex-end', 
        paddingRight: 10
    },
    imageLogout: {
        height: 25, 
        width: 25, 
        tintColor: '#fff'
    }
});

export default topNavBarStyles;