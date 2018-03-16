import { StyleSheet, Dimensions } from 'react-native';

export const chevronColor = '#2FAFCC';

const circleDiameter = Dimensions.get('window').width * 0.5;
const chevronHeight = 50;
const placeholderHeight = circleDiameter * 0.5;

export const layoutStylesForEmployeeDetailsScreen = StyleSheet.create({
    chevronPlaceholder: {
        height: placeholderHeight - chevronHeight * 0.5,
        backgroundColor: chevronColor
    }
});
