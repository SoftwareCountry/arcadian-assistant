import { StyleSheet, Platform } from 'react-native';

const topNavBarStyles = StyleSheet.create({
    headerView: {
        height: Platform.OS === 'ios' ? 70 : 50,
        marginTop: 0,
        backgroundColor: '#2FAFCC'
    },
    navTitle: {
        marginTop: Platform.OS === 'ios' ? 40 : 20,
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