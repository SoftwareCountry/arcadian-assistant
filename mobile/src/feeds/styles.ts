import { StyleSheet } from 'react-native';

export const screenStyles = StyleSheet.create({
    view: {
        flex: 1,
        backgroundColor: '#FFF',
        paddingLeft: 19,
        paddingRight: 19
    },
    viewHeaderText: {
        fontSize: 12
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
        paddingBottom: 5
    },
    imgContainer: {
        marginTop: 5,
        flex: 2
    },
    img: {
        flex: 1
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