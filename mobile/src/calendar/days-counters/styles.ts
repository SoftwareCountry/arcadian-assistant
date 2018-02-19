import { StyleSheet, ViewStyle } from 'react-native';

const daysCounterFontColor = '#18515E';
const daysCounterTitleColor = '#18515E';
const daysCounterPrimaryColor = '#2FAFCC';
const backgroundColor = 'rgba(47, 175, 204, 0.2)';

export const daysCountersStyles = StyleSheet.create({
    container: {
        flex: 2,
        flexDirection: 'row',
        height: 79
    }
});

export const daysCounterStyles = StyleSheet.create({
    container: {
        backgroundColor: backgroundColor,
        flexDirection: 'column',
        flex: 1
    },
    content: {
        flex: 1,
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center'
    },
    counter: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: 10
    },
    icon: {
        paddingRight: 10
    },
    contentValue: {
        fontSize: 26,
        lineHeight: 32,
        color: daysCounterFontColor
    },
    contentTitle: {
        fontSize: 10,
        lineHeight: 12,
        color: daysCounterFontColor
    }
});