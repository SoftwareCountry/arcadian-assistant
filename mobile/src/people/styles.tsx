import { StyleSheet } from 'react-native';

export const employeesListStyles  = StyleSheet.create({
    view: {
        flex: 1,
        backgroundColor: '#FFF'
    }
});

export const employeesListItemStyles = StyleSheet.create({
    layout: {
        height: 80,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
    },
    imgContainer: {
        width: 70,
        height: 70,
        marginTop: 5,
        marginLeft: 12
    },
    img: {
        width: 70,
        height: 70
    },
    info: {
        flex: 1,
        marginRight: 12,
        marginLeft: 12,
        flexDirection: 'column',
        justifyContent: 'center'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 9,
        textAlign: 'left'
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15
    }
});