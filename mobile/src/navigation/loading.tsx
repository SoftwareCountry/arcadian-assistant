import React from 'react';
import { View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { SearchView, SearchType } from './search-view';
import { searchViewStyles as styles } from './search-view-styles';

interface LoadingProps {
    type: SearchType;
}

export class LoadingView extends React.Component<LoadingProps> {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <View style={styles.searchContainer}>
                    <SearchView type={this.props.type}/>
                </View>
                <View style={styles.loadingTextContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
                </View>
            </View>
        );
    }
}