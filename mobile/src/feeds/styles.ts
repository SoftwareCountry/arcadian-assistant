import { StyleSheet } from 'react-native';

const screenPadding = 19;

export const screenStyles = StyleSheet.create({
    view: {
        flex: 1,
        backgroundColor: '#FFF'
    },
    viewHeaderText: {
        fontSize: 12,
        paddingLeft: screenPadding,
        paddingRight: screenPadding
    },
    separator: {
        height: 15
    }
});

export const feedStyles = StyleSheet.create({
    layout: {
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        paddingTop: 0,
        paddingBottom: 5,
        paddingLeft: screenPadding,
        paddingRight: screenPadding
    },
    imgContainer: {
        marginTop: 5,
        flex: 2,
        justifyContent: 'flex-start',
        alignItems: 'flex-start',
        flexDirection: 'row'
    },
    info: {
        flex: 6,
        flexDirection: 'column',
        alignSelf: 'flex-start',
        paddingLeft: 13
    },
    title: {
        fontSize: 19,
        textAlign: 'left',
        fontWeight: '400',
        letterSpacing: 2,
    },
    text: {
        fontSize: 15,
        textAlign: 'left',
        paddingTop: 2,
        paddingBottom: 2
    },
    tags: {
        color: '#2FAFCC',
        fontSize: 13
    },
    date: {
        fontSize: 12
    }
});