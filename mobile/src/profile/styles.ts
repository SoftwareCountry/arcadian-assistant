import { StyleSheet, ViewStyle } from 'react-native';

const daysCounterFontColor = '#fff';
const daysCounterTitleColor = '#18515E';
const daysCounterPrimaryColor = '#fff';
const chevronColor = '#2FAFCC';

const circleDiameter = 150;
const chevronHeight = 50;
const placeholderHeight = circleDiameter * .5 - chevronHeight * .5;

const daysCounterShapeZIndex = 2;

export const layoutStyles = StyleSheet.create({
    container: {
        flex: 1,
        alignSelf: 'stretch'
    },
    content: {
        flex: 1,
        flexDirection: 'column',
        marginTop: chevronHeight + placeholderHeight

    },
    avatarContainer: {
        borderRadius: circleDiameter / 2,
        height: circleDiameter,
        width: circleDiameter,
        zIndex: daysCounterShapeZIndex + 1,
        left: '50%',
        top: placeholderHeight * -1,
        position: 'absolute',
        transform: [{ translateX: circleDiameter * -.5 }],
        alignItems: 'center',
        justifyContent: 'center',
        //borderWidth: 2,
        //backgroundColor: '#fff',
        //borderColor: daysCounterPrimaryColor,
    },
    chevron: {
        width: '100%',
        backgroundColor: 'transparent',
        height: 0,
        zIndex: daysCounterShapeZIndex,
        borderBottomWidth: chevronHeight,
        borderStyle: 'solid',
        borderLeftColor: 'transparent',
        borderRightColor: 'transparent',
        borderBottomColor: chevronColor,
        position: 'absolute',
        transform: [
            { rotate: '180deg' }
        ]
    },
    chevronPlaceholder : {
        height: placeholderHeight,
        backgroundColor: chevronColor
    }
});

export const contentStyles = StyleSheet.create({
    name: {
        fontSize: 36,
        textAlign: 'center',
        color: '#000'
    },
    position: {
        fontSize: 13,
        textAlign: 'center',
        color: '#000'
    },
    department: {
        fontSize: 13,
        textAlign: 'center',
        color: '#2FAFCC',
        fontWeight: 'bold'
    },
    infoContainer: {
        flexDirection: 'row',
        flex: 1
    },
    contactsContainer: {}
});