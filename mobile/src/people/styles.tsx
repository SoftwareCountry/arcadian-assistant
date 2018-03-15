import { StyleSheet } from 'react-native';

export const employeesListStyles  = StyleSheet.create({
    view: {
        paddingTop: 5,
        flex: 1,
        backgroundColor: '#FFF'
    },
    loadingContainer: {
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center'
    },
    loadingText: {
        fontSize: 20
    }
});

export const employeesListItemStyles = StyleSheet.create({
    layout: {
        height: 80,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    avatarContainer: {
        width: 72,
        height: 72,
        marginLeft: 12,
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 2
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
        paddingTop: 2
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
        paddingBottom: 2
    }
});

export const companyItemStyles = StyleSheet.create({
    layout: {
        height: 90,
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center'
    },
    avatarContainer: {
        width: 56,
        height: 56,
        marginLeft: 52,
    },
    avatarOuterFrame: {
        borderWidth: 1
    },
    avatarImage: {
        borderWidth: 2
    },
    info: {
        flex: 1,
        height: 56,
        marginRight: 12,
        marginLeft: 12,
        flexDirection: 'column',
        justifyContent: 'flex-start',
        alignItems: 'flex-start'
    },
    baseText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 9,
        paddingTop: 2,
        paddingBottom: 10
    },
    depabbText: {
        fontFamily: 'Helvetica-Light',
        fontSize: 9,
        paddingTop: 2,
        color: '#2FAFCC',
        
    },
    name: {
        fontFamily: 'Helvetica-Light',
        fontSize: 15,
        paddingBottom: 2
    }
});