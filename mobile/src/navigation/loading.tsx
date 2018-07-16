import React from 'react';
import { View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { searchViewStyles as styles } from './search-view-styles';

export class LoadingView extends React.Component {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <StyledText style={styles.loadingText}>Loading...</StyledText>
            </View>
        );
    }
}