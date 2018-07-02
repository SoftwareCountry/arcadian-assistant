import React from 'react';
import { View } from 'react-native';
import { StyledText } from '../override/styled-text';
import { SearchView, SearchType } from './search-view';
import { ApplicationIcon } from '../override/application-icon';
import { searchViewStyles as styles } from './search-view-styles';

export class LoadingView extends React.Component {
    public render() {
        return (
            <View style={styles.loadingContainer}>
                <View style={styles.searchContainer}>
                    <View style={styles.container}>
                        <View style={styles.iconsContainer}>
                            <ApplicationIcon name={'search'} style={styles.icon} />
                        </View>
                        <View style={styles.inputContainer}>
                        </View>
                        <View style={styles.iconsContainer}>
                            <ApplicationIcon name={'reject-cross'} style={styles.icon} />
                        </View>
                    </View>
                </View>
                <View style={styles.loadingTextContainer}>
                    <StyledText style={styles.loadingText}>Loading...</StyledText>
                </View>
            </View>
        );
    }
}