import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';

import { Feed } from '../reducers/organization/feed.model';

interface FeedListItemProps {
    message: Feed;
    id: string;
}

const styles = StyleSheet.create({
    layout: {
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        paddingTop: 5,
        paddingBottom: 5,
        padding: 10,
        backgroundColor: '#acacac'
    },
    img: {
        flex: 1,
        width: 50,
        marginTop: 40,
        resizeMode: 'contain'
    },
    info: {
        flex: 2,
        flexDirection: 'column',
        alignSelf: 'center',
        padding: 20
    },
    title: {
        fontSize: 16,
        textAlign: 'left',
        fontWeight: '600'
    },
    text: {
        //marginTop: Platform.OS === 'ios' ? 40 : 20,
        fontSize: 14,
        textAlign: 'left',
    },
    date: {
        fontSize: 10
    }
});

export class FeedListItem extends React.Component<FeedListItemProps> {
    public render() {
        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <Image style={styles.img} source={{uri: 'https://image.flaticon.com/icons/png/128/126/126486.png'}}/>
                    <View style={styles.info}>
                        <Text style={styles.title}>{this.props.message.title}</Text>
                        <Text style={styles.text}>{this.props.message.text}</Text>
                        <Text style={styles.date}>{this.props.message.datePosted}</Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}