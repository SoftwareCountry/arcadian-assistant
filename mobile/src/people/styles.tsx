import { StyleSheet } from 'react-native';

export const employeesListStyles  = StyleSheet.create({
    view: {
        flex: 1,
        backgroundColor: '#FFF'
    }
});

export const employeesListItemStyles = StyleSheet.create({
    layout: {
        height: 36,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
    },
    imgContainer: {
        marginTop: 5,
        marginLeft: 28,
        flex: 1
    },
    img: {
        width: 25,
        height: 25
    },
    info: {
        flex: 6,
        marginRight: 28,
        justifyContent: 'center'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 12,
        textAlign: 'left'
    },
    name: {
        fontWeight: 'bold'
    }
});