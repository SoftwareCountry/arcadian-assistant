import { StyleSheet, ViewStyle } from 'react-native';

const counterColor = '#18515E';
const labelColor = counterColor;
const backgroundColor = 'rgba(47, 175, 204, 0.2)';
const iconColor = counterColor;

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
    counterContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: 10
    },
    icon: {
        paddingRight: 10,
        color: iconColor,
        opacity: .5
    },
    counter: {
        fontSize: 26,
        lineHeight: 32,
        color: counterColor
    },
    label: {
        fontSize: 10,
        lineHeight: 12,
        color: labelColor
    }
});